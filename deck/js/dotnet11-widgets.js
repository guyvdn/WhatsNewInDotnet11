/* ==========================================================================
   Deck-specific widgets for "What's new in .NET 11". Loaded before chrome.js.
   ========================================================================== */

/* --------------------------------------------------------------------------
   exhaustive — a fake editor with a non-exhaustive switch over a union.
   The light bulb adds the missing case; CS8509 clears and the build goes green.
   The diagnostic text is copied from a real build on 11.0.100 (GA).
   -------------------------------------------------------------------------- */
DeckWidgets.register('exhaustive', function (el) {
  const { $ } = DeckWidgets.helpers;
  const section = el.closest('section');
  const editor = $(el, 'editor');
  const errors = $(el, 'errors');
  const build = $(el, 'build');

  const LANG = 'csharp';
  const BAD_LINE = 3;                 // where CS8509 lands: the 'switch' keyword
  const BEFORE = [
    [1, 'public union Pet(Cat, Dog, Bird);'],
    [2, ''],
    [3, 'string Describe(Pet pet) => pet switch'],
    [4, '{'],
    [5, '    Dog d => $"{d.Name} barks",'],
    [6, '    Cat c => $"{c.Name} purrs",'],
    [7, '};']
  ];
  const ADDED = [7, '    Bird b => $"{b.Name} sings",'];
  const ERR_BAD = "<b>CS8509</b>&nbsp; The switch expression does not handle all possible values of its " +
                  "input type (it is not exhaustive). For example, the pattern 'Bird' is not covered.";
  const ERR_OK = 'every case of <b>Pet</b> handled &mdash; no <code>_ =&gt;</code> case needed';
  const FIXES = ['Add missing cases'];
  const FIX_HDR = 'CS8509 &nbsp;&middot;&nbsp; Program.cs';

  let fixed = false;

  function lines() {
    if (!fixed) return BEFORE;
    const out = BEFORE.slice(0, 6).concat([ADDED, [8, '};']]);
    return out;
  }

  function paint() {
    editor.innerHTML = lines().map(([no, text]) => {
      let src = DeckCode.tokenize(text, LANG);
      if (no === BAD_LINE && !fixed) {
        src = src.replace('switch', '<span class="squig warn">switch</span>');
      }
      return `<div class="row" data-line="${no}">` +
             `<span class="gut">${no}</span>` +
             `<span class="src">${src || ' '}</span></div>`;
    }).join('');

    if (fixed) return;
    const row = editor.querySelector(`.row[data-line="${BAD_LINE}"]`);
    const bulb = document.createElement('span');
    bulb.className = 'bulb';
    bulb.title = 'Show potential fixes';
    bulb.textContent = '\u{1F4A1}';
    row.appendChild(bulb);
    bulb.addEventListener('click', openMenu);
    const squig = row.querySelector('.squig');
    if (squig) squig.addEventListener('click', openMenu);
  }

  const closeMenu = () => {
    const m = editor.querySelector('.fixmenu');
    if (m) m.remove();
  };

  function openMenu(ev) {
    ev.stopPropagation();
    closeMenu();
    const row = editor.querySelector(`.row[data-line="${BAD_LINE}"]`);
    const menu = document.createElement('div');
    menu.className = 'fixmenu';
    menu.innerHTML = `<div class="hdr">${FIX_HDR}</div>` +
      FIXES.map((f) => `<button>\u{1F4A1} ${f}</button>`).join('');
    menu.style.left = '52px';
    menu.style.top = (row.offsetTop + row.offsetHeight + 4) + 'px';
    editor.appendChild(menu);
    menu.querySelectorAll('button').forEach((b) => b.addEventListener('click', applyFix));
  }

  function applyFix(ev) {
    if (ev) ev.stopPropagation();
    closeMenu();
    fixed = true;
    paint();
    const src = editor.querySelector('.row[data-line="7"] .src');
    if (src) {
      src.style.transition = 'background .6s ease';
      src.style.background = 'color-mix(in oklab, var(--ok) 22%, transparent)';
      setTimeout(() => { src.style.background = 'transparent'; }, 700);
    }
    errors.classList.add('clean');
    errors.innerHTML = '<span class="sev">✓ 0 warnings</span><span>' + ERR_OK + '</span>';
    build.classList.remove('amber');
    build.classList.add('green');
    $(build, 'build-icon').innerHTML = '✓';
    $(build, 'build-text').textContent = 'Build succeeded';
  }

  function reset() {
    fixed = false;
    closeMenu();
    paint();
    errors.classList.remove('clean');
    errors.innerHTML = '<span class="sev warn">⚠ warning</span><span>' + ERR_BAD + '</span>';
    build.classList.remove('green');
    build.classList.add('amber');
    $(build, 'build-icon').innerHTML = '⚠';
    $(build, 'build-text').textContent = '1 warning';
  }

  editor.addEventListener('click', closeMenu);
  const r = $(section, 'reset');
  if (r) r.addEventListener('click', reset);
  reset();
  DeckWidgets.own(section, { run: () => (fixed ? reset() : applyFix()), reset });
});
