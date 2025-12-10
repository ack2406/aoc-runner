package main

import (
	"fmt"
	"io"
	"os"
	"strconv"
	"strings"
)

func parseInput(raw string) string {
	return strings.TrimSpace(raw)
}

func part1(input string) string {
	return ""
}

func part2(input string) string {
	return ""
}

func main() {
	data, _ := io.ReadAll(os.Stdin)
	input := parseInput(string(data))

	part := 0
	if len(os.Args) > 1 {
		if p, err := strconv.Atoi(strings.TrimSpace(os.Args[1])); err == nil {
			part = p
		}
	}

	switch part {
	case 1:
		fmt.Println(part1(input))
	case 2:
		fmt.Println(part2(input))
	default:
		fmt.Printf("%s\n%s\n", part1(input), part2(input))
	}
}
