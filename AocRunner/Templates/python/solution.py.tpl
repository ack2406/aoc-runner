# Advent of Code {{year}} - Day {{day}}


def parse_input(raw: str):
    return [line.strip() for line in raw.strip().splitlines() if line.strip()]


def part1(data) -> int:
    return 0


def part2(data) -> int:
    return 0


def solve(raw: str, part: int | None = None) -> str:
    data = parse_input(raw)

    if part == 1:
        return str(part1(data))
    if part == 2:
        return str(part2(data))

    return f"{part1(data)}\n{part2(data)}"


if __name__ == "__main__":
    import sys

    cli_part = int(sys.argv[1]) if len(sys.argv) > 1 and sys.argv[1].strip() else None
    print(solve(sys.stdin.read(), cli_part))
