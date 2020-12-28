using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pidgin;
using Pidgin.Expression;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace AoC {
    public class MathParser {
        public interface IExpr : IEquatable<IExpr> {
            long Evaluate();
        }

        public class Identifier : IExpr {
            public string Name { get; }

            public Identifier(string name) {
                Name = name;
            }

            public bool Equals(IExpr other)
                => other is Identifier i && this.Name == i.Name;

            public long Evaluate() {
                throw new NotImplementedException();
            }
        }

        public class Literal : IExpr {
            public long Value { get; }

            public Literal(long value) {
                Value = value;
            }

            public bool Equals(IExpr other)
                => other is Literal l && this.Value == l.Value;

            public long Evaluate() {
                return Value;
            }
        }

        public enum UnaryOperatorType {
            Neg,
            Complement
        }

        public class UnaryOp : IExpr {
            public UnaryOperatorType Type { get; }
            public IExpr Expr { get; }

            public UnaryOp(UnaryOperatorType type, IExpr expr) {
                Type = type;
                Expr = expr;
            }

            public bool Equals(IExpr other)
                => other is UnaryOp u
                && this.Type == u.Type
                && this.Expr.Equals(u.Expr);

            public long Evaluate() {
                if (Type == UnaryOperatorType.Neg)
                    return -Expr.Evaluate();
                throw new NotImplementedException();
            }
        }

        public enum BinaryOperatorType {
            Add,
            Mul
        }

        public class BinaryOp : IExpr {
            public BinaryOperatorType Type { get; }
            public IExpr Left { get; }
            public IExpr Right { get; }

            public BinaryOp(BinaryOperatorType type, IExpr left, IExpr right) {
                Type = type;
                Left = left;
                Right = right;
            }

            public bool Equals(IExpr other)
                => other is BinaryOp b
                && this.Type == b.Type
                && this.Left.Equals(b.Left)
                && this.Right.Equals(b.Right);

            public long Evaluate() {
                if (Type == BinaryOperatorType.Add)
                    return Left.Evaluate() + Right.Evaluate();
                if (Type == BinaryOperatorType.Mul)
                    return Left.Evaluate() * Right.Evaluate();
                throw new NotImplementedException();
            }
        }

        public static class ExprParser {
            private static Parser<char, T> Tok<T>(Parser<char, T> token) => Try(token).Before(SkipWhitespaces);
            private static Parser<char, string> Tok(string token) => Tok(String(token));

            private static Parser<char, T> Parenthesised<T>(Parser<char, T> parser) => parser.Between(Tok("("), Tok(")"));

            private static Parser<char, Func<IExpr, IExpr, IExpr>> Binary(Parser<char, BinaryOperatorType> op)
                => op.Select<Func<IExpr, IExpr, IExpr>>(type => (l, r) => new BinaryOp(type, l, r));
            private static Parser<char, Func<IExpr, IExpr>> Unary(Parser<char, UnaryOperatorType> op)
                => op.Select<Func<IExpr, IExpr>>(type => o => new UnaryOp(type, o));

            private static readonly Parser<char, Func<IExpr, IExpr, IExpr>> Add = Binary(Tok("+").ThenReturn(BinaryOperatorType.Add));
            private static readonly Parser<char, Func<IExpr, IExpr, IExpr>> Mul = Binary(Tok("*").ThenReturn(BinaryOperatorType.Mul));
            private static readonly Parser<char, Func<IExpr, IExpr>> Neg = Unary(Tok("-").ThenReturn(UnaryOperatorType.Neg));
            private static readonly Parser<char, Func<IExpr, IExpr>> Complement = Unary(Tok("~").ThenReturn(UnaryOperatorType.Complement));

            private static readonly Parser<char, IExpr> Identifier = Tok(Letter.Then(LetterOrDigit.ManyString(), (h, t) => h + t))
                    .Select<IExpr>(name => new Identifier(name))
                    .Labelled("identifier");
            private static readonly Parser<char, IExpr> Literal = Tok(LongNum)
                    .Select<IExpr>(value => new Literal(value))
                    .Labelled("longeger literal");

            //private static Parser<char, Func<IExpr, IExpr>> Call(Parser<char, IExpr> subExpr)
            //    => Parenthesised(subExpr.Separated(Tok(",")))
            //        .Select<Func<IExpr, IExpr>>(args => method => new Call(method, args.ToImmutableArray()))
            //        .Labelled("function call");

            private static readonly Parser<char, IExpr> Expr2 = ExpressionParser.Build<char, IExpr>(
                expr => (
                    OneOf(
                        Identifier,
                        Literal,
                        Parenthesised(expr).Labelled("parenthesised expression")
                    ),
                    new[] {
                        Operator.Prefix(Neg).And(Operator.Prefix(Complement)),
                        Operator.InfixL(Add),
                        Operator.InfixL(Mul),
                    }
                )
            ).Labelled("expression");

            private static readonly Parser<char, IExpr> Expr1 = ExpressionParser.Build<char, IExpr>(
                expr => (
                    OneOf(
                        Identifier,
                        Literal,
                        Parenthesised(expr).Labelled("parenthesised expression")
                    ),
                    new[] {
                        Operator.Prefix(Neg).And(Operator.Prefix(Complement)),
                        Operator.InfixL(Add).And(Operator.InfixL(Mul)),
                    }
                )
            ).Labelled("expression");

            public static IExpr ParsePart1(string input) => Expr1.ParseOrThrow(input);

            public static IExpr ParsePart2(string input) => Expr2.ParseOrThrow(input);
        }
    }
}
