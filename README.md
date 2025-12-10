# aoc-runner

Small CLI to run and scaffold Advent of Code solutions across multiple languages (Python, Go, Scala).

## Related repo
- Solutions live in `ack2406/aoc-solutions`: https://github.com/ack2406/aoc-solutions

## Requirements
- .NET 8+ SDK (builds `net10.0` target).
- Language runtimes as needed:
  - Python solutions: `python3`
  - Go solutions: `go`
  - Scala solutions: `scala-cli` (preferred) or `scala`
- For real inputs: an Advent of Code session token in `config.json`.

## Setup
1) Clone both repos side by side (default config assumes `../aoc-solutions`):
```
projects/
  aoc-runner
  aoc-solutions
```
2) Build (restores templates into output):
```
dotnet build
```
3) Check or edit `config.json` (created automatically on first run):
```json
{
  "SolutionPath": "../aoc-solutions",
  "SessionToken": "session=...",
  "CacheDir": ".cache"
}
```

## Usage
All commands accept the `--project AocRunner` pattern when running from the repo root:

- Run a solution (optionally pick part or sample input):
```
dotnet run --project AocRunner -- run 2024 1
dotnet run --project AocRunner -- run 2024 1 2
dotnet run --project AocRunner -- run 2024 1 --sample
```
- Scaffold a new day (creates `dayXX` with meta.json, sample.txt, and template):
```
dotnet run --project AocRunner -- new 2025 11 --lang go
dotnet run --project AocRunner -- new 2025 12 --lang scala
dotnet run --project AocRunner -- new 2025 13 --lang python
```

## Expected solution layout
Each day lives at `<SolutionPath>/<year>/dayXX/` with:
- `meta.json` (language, entry point, year/day)
- `sample.txt`
- entry point file (e.g., `solution.py`, `solution.go`, `solution.scala`)

Example `meta.json`:
```json
{
  "Year": 2024,
  "Day": 1,
  "Language": "go",
  "EntryPoint": "solution.go"
}
```

## Notes
- Scala runner tries `scala-cli` first, then `scala`.
- Input downloads respect `CacheDir`; pass `--sample` to avoid network usage.
- Templates live under `AocRunner/Templates/<language>/`.
