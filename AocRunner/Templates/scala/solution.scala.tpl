import scala.util.Try
import scala.io.Source

object Solution {
  private def parseInput(raw: String): List[String] =
    raw.trim.split("\\n").toList

  def part1(input: List[String]): String = ""

  def part2(input: List[String]): String = ""

  def main(args: Array[String]): Unit = {
    val input = parseInput(Source.stdin.mkString)
    val part = args.headOption.flatMap(arg => Try(arg.trim.toInt).toOption)

    part match {
      case Some(1) => println(part1(input))
      case Some(2) => println(part2(input))
      case _ =>
        println(part1(input))
        println(part2(input))
    }
  }
}
