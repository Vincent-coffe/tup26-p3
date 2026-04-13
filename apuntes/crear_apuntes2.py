#!/usr/bin/env python3

from __future__ import annotations

import datetime as dt
import importlib
import re
import sys
import unicodedata
import zipfile
from pathlib import Path
from typing import Any
from xml.sax.saxutils import escape

try:
    Markdown = importlib.import_module("markdown").Markdown
    HtmlFormatter = importlib.import_module("pygments.formatters").HtmlFormatter
except ModuleNotFoundError as exc:
    raise SystemExit(
        "Faltan dependencias para generar el EPUB. Instala Markdown y Pygments, por ejemplo con: uv pip install Markdown Pygments"
    ) from exc


ROOT = Path(__file__).resolve().parent
OUTPUT = ROOT / "apuntes-programacion-iii.epub"
BOOK_ID = "apuntes-programacion-iii"
BOOK_TITLE = "Apuntes de Programacion III"
BOOK_LANGUAGE = "es"
BOOK_SUBTITLE = "C#, .NET y herramientas de desarrollo"
BOOK_AUTHOR = "Adrián Di Battista"
BOOK_COVER = ROOT / "portada.jpg"
EXCLUDED = {"00.010-programa-de-programacion-iii.md"}


def slugify(text: str) -> str:
    normalized = unicodedata.normalize("NFKD", text)
    ascii_only = normalized.encode("ascii", "ignore").decode("ascii")
    slug = re.sub(r"[^a-zA-Z0-9]+", "-", ascii_only).strip("-").lower()
    return slug or "section"


def markdown_slugify(value: str, separator: str, unicode: bool = False) -> str:
    del unicode
    return slugify(value).replace("-", separator)


def first_heading(markdown_text: str, fallback: str) -> str:
    for line in markdown_text.splitlines():
        stripped = line.strip()
        if stripped.startswith("# "):
            return stripped[2:].strip()
    return fallback


def render_inline_markdown(text: str) -> str:
    rendered = Markdown(extensions=["extra"], output_format="xhtml").convert(text).strip()
    if rendered.startswith("<p>") and rendered.endswith("</p>"):
        return rendered[3:-4]
    return rendered


def strip_leading_title(markdown_text: str, chapter_title: str) -> str:
    lines = markdown_text.splitlines()
    for index, line in enumerate(lines):
        stripped = line.strip()
        if not stripped:
            continue
        if stripped == f"# {chapter_title}" or stripped.startswith("# "):
            remainder = lines[index + 1 :]
            while remainder and not remainder[0].strip():
                remainder = remainder[1:]
            return "\n".join(remainder)
        break
    return markdown_text


def build_markdown_renderer() -> Any:
    return Markdown(
        extensions=["extra", "toc", "codehilite"],
        extension_configs={
            "toc": {
                "slugify": markdown_slugify,
                "permalink": False,
            },
            "codehilite": {
                "css_class": "codehilite",
                "guess_lang": False,
                "noclasses": False,
                "use_pygments": True,
            },
        },
        output_format="xhtml",
    )


def wrap_xhtml_page(title: str, body: str, *, nav: bool = False) -> str:
    nav_attr = ' xmlns:epub="http://www.idpf.org/2007/ops"'
    return f"""<?xml version="1.0" encoding="utf-8"?>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml"{nav_attr} xml:lang="{BOOK_LANGUAGE}">
  <head>
    <title>{escape(title)}</title>
    <link rel="stylesheet" type="text/css" href="styles.css" />
  </head>
  <body>
    {body}
  </body>
</html>
"""


def build_cover_page() -> str:
    body = """
<section epub:type="cover" class="cover-page">
  <div class="cover-frame">
    <img src="portada.jpg" alt="Portada de Apuntes de Programacion III" />
  </div>
</section>
"""
    return wrap_xhtml_page("Portada", body)


def markdown_to_xhtml(markdown_text: str, chapter_title: str, chapter_number: int) -> str:
    markdown_text = strip_leading_title(markdown_text, chapter_title)
    body = build_markdown_renderer().convert(markdown_text).strip()
    chapter_body = f"""
<section epub:type="chapter">
  <header class="chapter-header">
    <p class="chapter-kicker">Capitulo {chapter_number}</p>
    <h1>{render_inline_markdown(chapter_title)}</h1>
  </header>
  {body}
</section>
"""
    return wrap_xhtml_page(chapter_title, chapter_body)


def build_epub(markdown_files: list[Path]) -> None:
    now = dt.datetime.now(dt.timezone.utc).replace(microsecond=0).isoformat()
    pygments_css = HtmlFormatter().get_style_defs(".codehilite")
    css = f"""
body {{ font-family: serif; line-height: 1.45; margin: 5%; }}
h1, h2, h3, h4, h5, h6 {{ line-height: 1.2; margin-top: 1.2em; }}
code {{ font-family: monospace; }}
pre, .codehilite pre {{ white-space: pre-wrap; overflow-wrap: anywhere; line-height: 1.5; }}
.codehilite {{ margin: 1em 0; }}
blockquote {{ border-left: 0.25em solid #999; margin-left: 0; padding-left: 1em; color: #444; }}
hr {{ border: none; border-top: 1px solid #bbb; margin: 1.5em 0; }}
.chapter-header {{ margin-bottom: 2.5em; padding-bottom: 0.8em; border-bottom: 1px solid #bbb; }}
.chapter-kicker {{ margin: 0; text-transform: uppercase; letter-spacing: 0.08em; font-size: 0.8em; color: #666; }}
.book-title {{ text-align: center; margin-top: 20%; }}
.toc-list li {{ margin: 0.4em 0; }}
.cover-page {{ margin: 0; padding: 0; }}
.cover-frame {{ margin: 0 auto; text-align: center; }}
.cover-frame img {{ display: block; width: 100%; height: auto; }}
{pygments_css}
"""

    chapters: list[tuple[str, str, str]] = []
    for index, path in enumerate(markdown_files, start=1):
        source = path.read_text(encoding="utf-8")
        title = first_heading(source, path.stem)
        chapter_file = f"chapter-{index:02d}.xhtml"
        xhtml = markdown_to_xhtml(source, title, index)
        chapters.append((chapter_file, title, xhtml))

    toc_items = "\n".join(
        f'        <li><a href="{filename}">Capitulo {index}: {escape(title)}</a></li>'
        for index, (filename, title, _) in enumerate(chapters, start=1)
    )

    index_body = f"""
<section epub:type="frontmatter toc">
  <div class="book-title">
    <h1>{escape(BOOK_TITLE)}</h1>
    <p>Indice general</p>
  </div>
  <nav epub:type="toc" id="toc">
    <ol class="toc-list">
{toc_items}
    </ol>
  </nav>
</section>
"""
    nav_xhtml = wrap_xhtml_page("Indice", index_body, nav=True)
    cover_xhtml = build_cover_page()

    manifest_items = [
        '    <item id="cover" href="cover.xhtml" media-type="application/xhtml+xml"/>',
        '    <item id="cover-image" href="portada.jpg" media-type="image/jpeg" properties="cover-image"/>',
        '    <item id="nav" href="nav.xhtml" media-type="application/xhtml+xml" properties="nav"/>',
        '    <item id="css" href="styles.css" media-type="text/css"/>',
    ]
    for index, (filename, _, _) in enumerate(chapters, start=1):
        manifest_items.append(
            f'    <item id="chap{index}" href="{filename}" media-type="application/xhtml+xml"/>'
        )

    spine_items = ['    <itemref idref="cover"/>', '    <itemref idref="nav"/>']
    for index in range(1, len(chapters) + 1):
        spine_items.append(f'    <itemref idref="chap{index}"/>')

    opf = f"""<?xml version="1.0" encoding="utf-8"?>
<package xmlns="http://www.idpf.org/2007/opf" unique-identifier="bookid" version="3.0">
  <metadata xmlns:dc="http://purl.org/dc/elements/1.1/">
    <dc:identifier id="bookid">{escape(BOOK_ID)}</dc:identifier>
    <dc:title>{escape(BOOK_TITLE)}</dc:title>
    <dc:language>{BOOK_LANGUAGE}</dc:language>
    <dc:creator>{escape(BOOK_AUTHOR)}</dc:creator>
    <dc:date>{now}</dc:date>
  </metadata>
  <manifest>
{chr(10).join(manifest_items)}
  </manifest>
  <spine>
{chr(10).join(spine_items)}
  </spine>
</package>
"""

    container_xml = """<?xml version="1.0" encoding="utf-8"?>
<container version="1.0" xmlns="urn:oasis:names:tc:opendocument:xmlns:container">
  <rootfiles>
    <rootfile full-path="OEBPS/content.opf" media-type="application/oebps-package+xml"/>
  </rootfiles>
</container>
"""

    with zipfile.ZipFile(OUTPUT, "w") as epub:
        epub.writestr(
            "mimetype",
            "application/epub+zip",
            compress_type=zipfile.ZIP_STORED,
        )
        epub.writestr("META-INF/container.xml", container_xml)
        epub.writestr("OEBPS/styles.css", css)
        epub.writestr("OEBPS/portada.jpg", BOOK_COVER.read_bytes())
        epub.writestr("OEBPS/cover.xhtml", cover_xhtml)
        epub.writestr("OEBPS/nav.xhtml", nav_xhtml)
        epub.writestr("OEBPS/content.opf", opf)
        for filename, _, xhtml in chapters:
            epub.writestr(f"OEBPS/{filename}", xhtml)


def main() -> int:
    markdown_files = sorted(
        path
        for path in ROOT.glob("*.md")
        if path.name not in EXCLUDED
    )
    if not markdown_files:
        print("No se encontraron archivos Markdown para incluir.", file=sys.stderr)
        return 1
    if not BOOK_COVER.exists():
        print(f"No se encontro la portada: {BOOK_COVER.name}", file=sys.stderr)
        return 1

    build_epub(markdown_files)
    print(OUTPUT.name)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
