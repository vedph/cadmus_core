# Date

## Elements
- Y year: `NNNN` (1000-current year).
- M month (1-12). It is `N` or `NN` or `aaa.` (`gen.`, `feb.`, `mar.`, `apr.`, `mag.`, `giu.`, `lug.`, `ago.`, `set.`, `ott.`, `nov.`, `dic.`) or full name (`gennaio`, `febbraio`, `marzo`, `aprile`, `maggio`, `giugno`, `luglio`, `agosto`, `settembre`, `ottobre`, `novembre`, `dicembre`).
- D day: `N` or `NN` (1-31).
- E decade: `NNNN` or `'NN` or `dieci`, `venti`, `trenta`, `quaranta`, `cinquanta`, `sessanta`, `settanta`, `ottanta`, `novanta` (case insensitive).
- C century: uppercase Roman number.
- X `senza data` or `s.d.` = null date. It cannot combine.

## Modifiers
- `[]` include deduced elements or combinations.

YMD prefix modifiers:

- `ante` ...
- `post` ...
- `circa`, `ca`, `ca.` ...

Century prefix modifiers:

- `inizio`
- `I metà`
- `metà`
- `II metà`
- `fine`

Century postfix modifiers:

- `ex.` = `fine`
- `in.` = `inizio`

## Combinations
YMD combinations:

- YMD
- YM
- Y
- DMY
- MY

Decade (cannot combine with other elements) combinations:

- `anni` + `NNNN` 
- `anni` + `'NN` 
- `anni` + name

Century (cannot combine with other elements) combinations:

- `secolo` or `sec.` + `R`
- `R` + `secolo` or `sec.`.

## Ranges

Range separators (for all the components combinations except null dates):

- ` - `
- `/`
- `-` only when this is not used in Y-M-D, Y-M, D-M-Y, M-Y
- `tra il` A `e il` B

Compendiary ranges: the second element (b) may miss M and/or D when equal to the first one (a), e.g.:

```
1987, mag. 23 – giu. 13 = YMD-MD
1987, mag. 23 – 25 = YMD-D
1987, mag. - giu. = YM-M
```

Several ranges are divided by `;` or `,`, e.g. `1987-1988; 1991-1992`.

## Samples

```
1957, mag. 21
1957, mag.
1957
1957, maggio 21
1957, maggio
21 mag. 1957
21 maggio 1957
mag. 1957
maggio 1957
1957/05/21
1957/05
1957.05.21
1957.05
1957-05-21
1957-05
21/05/1957
05/1957
21.05.1957
05.1957
21-05-1957
05-1957
```

Ranges:

```
1987, mag. 23 - 1987, mag. 25
1987, mag. 23 - 1987, giu. 30
```

Deductions:

```
1957, mag. [21]
1957, [mag. 21]
[1957, mag. 21]
[1957, mag.] 21
[1957], mag. [21]
1957, [mag.]
[1957, mag.]
[1957]
```

## Procedure

1. split at `;` (TODO: prefilter to replace `,` as ranges separator with `;`). Each part is a full date or dates range.

2. determine the M style (Numeric, ShortName, FullName): match any of the month names, full or abbreviated; if none matched, assume numeric.

3. if the M style is numeric, determine the YMD separator: `-` or `/`. If not explicitly specified, match:
- for slash, `(?:/\d{1,2}/)|(?:(?<!-)[12]\d{3}/\d{1,2}\b)`;
- for dash, `(?:-\d{1,2}-)|(?:(?<!/)[12]\d{3}-\d{1,2}\b)`;
Any other separator (e.g. dot) is not relevant. The first expression matching wins.

4. determine the YMD order: YMD or DMY. Order is DMY if it matches `\b\d{1,2}[-/\.][12]\d{3}[^\p{L}\d]*$`; else assume YMD.

5. split at range separator, which is ` - `, ` / `, or `/` if YMD separator is `-`, `-` if YMD separator is `/`.

Patterns:

DMY, only Y required
- DMY + M-style=N: 20-12-1923
- DMY + M-style=S: 20 dic.1923
- DMY + M-style=F: 20 dicembre 1923

YMD, only Y required
- YMD + M-style=N: 1923-12-20
- YMD + M-style=S: 1923, dic. 20
- YMD + M-style=F: 1923, dicembre 21

## Test samples

```
; NULL
senza data
s.d.

; CENTURIES
sec. XX
XX sec.
secolo XX
XX secolo

inizio sec. XX
I metà sec. XX
metà sec. XX
II metà sec. XX
fine sec. XX

inizio XX sec.
I metà XX sec.
metà XX sec.
II metà XX sec.
fine XX sec.

sec. XX ex.
sec. XX in.
XX sec. ex.
XX sec. in.

; DECADES
anni '10
anni '20
anni '30
anni '40
anni '50
anni '60
anni '70
anni '80
anni '90

anni 1910
anni 1920
anni 1930
anni 1940
anni 1950
anni 1960
anni 1970
anni 1980
anni 1990

anni dieci
anni Dieci
anni venti
anni trenta
anni quaranta
anni cinquanta
anni sessanta
anni settanta
anni ottanta
anni novanta

; YMD, mstyle=N, sep=dash
1970-05-30
1970-5-30
1970-05
1970

[1970]-05-30
1970-[05]-30
1970-05-[30]
[1970-05]-30
1970-[05-30]
[1970-05-30]

[1970-05]
[1970]-05
1970-[05]
[1970]

ca. 1970-05-30
ca 1970-5-30
circa 1970-05
ca. 1970

; YMD, mstyle=S
1970, mag. 30
1970, mag.
1970, lu. 30

[1970], mag. 30
1970, [mag.] 30
1970, mag. [30]
[1970, mag.]-30
1970, [mag. 30]
[1970, mag. 30]

[1970], mag.
1970, [mag.]
[1970, mag.]

ca. 1970, mag. 30
ca 1970, mag. 30
circa 1970, mag.

; YMD, mstyle=F
1970, maggio 30
1970, maggio

[1970], maggio 30
1970, maggio 30
1970, maggio [30]
[1970, maggio] 30
1970, [maggio 30]
[1970, maggio 30]

[1970, maggio]
[1970], maggio
1970, [maggio]

ca. 1970, maggio 30
ca 1970, maggio 30
circa 1970, maggio

; YMD, mstyle=N, sep=slash
1970/05/30
1970/5/30
1970/05

[1970]/05/30
1970/[05]/30
1970/05/[30]
[1970/05]/30
1970/[05/30]
[1970/05/30]

[1970/05]
[1970]/05
1970/[05]

ca. 1970/05/30
ca 1970/5/30
circa 1970/05

; DMY, mstyle=N, sep=dash
30-05-1970
30-5-1970
05-1970

[30]-05-1970
30-[05]-1970
30-05-[1970]
[30-05]-1970
30-[05-1970]
[30-05-1970]

[05-1970]
05-[1970]
[05]-1970

ca. 30-05-1970
ca 30-5-1970
circa 05-1970

; DMY, mstyle=S
30 mag. 1970
mag. 1970

30 mag. [1970]
30 [mag.] 1970
[30] mag. 1970
30 [mag. 1970]
[30 mag.] 1970
[30 mag. 1970]

[mag. 1970]
[mag.] 1970
mag. [1970]

ca. 30 mag. 1970
ca 30 mag. 1970
circa mag. 1970

; DMY, mstyle=F
30 maggio 1970
maggio 1970

30 maggio [1970]
30 [maggio] 1970
[30] maggio 1970
30 [maggio 1970]
[30 maggio] 1970
[30 maggio 1970]

[maggio 1970]
[maggio] 1970
maggio [1970]

ca. 30 maggio 1970
ca 30 maggio 1970
circa maggio 1970

; TERMINUS ANTE
ante 1970
ante 1970-05-30
ante 1970-05
ante 30-05-1970
ante 05-1970
ante 1970, mag. 30
ante 1970, maggio 30
ante 30 mag. 1970
ante 30 maggio 1970

; TERMINUS POST
post 1970
post 1970-05-30
post 1970-05
post 30-05-1970
post 05-1970
post 1970, mag. 30
post 1970, maggio 30
post 30 mag. 1970
post 30 maggio 1970

; RANGES
1970 - 1980
1970 / 1980
1970-05 - 1970-06
1970-05-30 - 1970-06-28
1970-05 / 1970-06
1970-05-30 / 1970-06-28
1970/05 - 1970/06
1970/05/30 - 1970/06/28
1970-05-30/1970-06-28
1970-05-30/1970-06-28
tra il 1970-05-30 e il 1970-06-28

; SHORTENED RANGES
; YMD style N
1970-05-30 / 06-28
1970-05-30 / 31
1970-05 / 06

; DMY style N
30-05 / 28-06-1970
30 / 31-05-1970
05 / 06-1970

; YMD style S
1970, mag. 30 / giu. 28
1970, mag. 30 / 31
1970, mag. / giu.

; DMY style S
30 mag. / 28 giu. 1970
30 / 31 mag. 1970
mag. / giu. 1970

; YMD style F
1970, maggio 30 / giugno 28
1970, maggio 30 / 31
1970, maggio / giugno

; DMY style F
30 maggio / 28 giugno 1970
30 / 31 maggio 1970
maggio / giugno 1970

; MULTIPLE
1970-05-30; 1973-06-01
```
