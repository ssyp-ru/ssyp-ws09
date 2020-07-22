// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using TheGrapho.Parser.Syntax;
using TheGrapho.Parser.Utilities;

namespace TheGrapho.Parser
{
    public sealed class Scanner
    {
        public Scanner([DisallowNull] string source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        [NotNull] private StringBuilder Scratch { get; } = new StringBuilder();
        [NotNull] private string Source { get; }
        [MaybeNull] private SyntaxNode? CurrentNode { get; set; }
        private int CurrentCharOffset { get; set; }
        private ScannerState State { get; set; }

        private char CurrentChar => Source[CurrentCharOffset];

        [return: MaybeNull]
        private SyntaxNode Scan()
        {
            State = (SafePeekChar(), SafePeekCharP1()) switch
            {
                var (c1, _) when c1.HasValue && CharacterUtilities.IsWhitespace(c1.Value) => ScannerState.Whitespace,
                ('<', _) => ScannerState.Html,
                ('/', '/') => ScannerState.LineComment,
                ('/', '*') => ScannerState.BlockComment,
                ('#', _) => ScannerState.Preprocessor,

                var (c1, c2) when c1.HasValue
                                  && c2.HasValue
                                  && Grammar.Punctuation.ContainsKey($"{c1.Value}{c2.Value}") =>
                ScannerState.Punctuation2,

                var (c1, _) when c1.HasValue && Grammar.Punctuation.ContainsKey($"{c1.Value}") => ScannerState
                    .Punctuation1,

                ('"', _) => ScannerState.String,
                var (c1, _) when c1.HasValue && char.IsDigit(c1.Value) => ScannerState.Number,
                ('.', var c2) when c2.HasValue && char.IsDigit(c2.Value) => ScannerState.Number,
                ('-', var c2) when c2.HasValue && (char.IsDigit(c2.Value) || c2.Value == '.') => ScannerState.Number,

                var (c1, _) when c1.HasValue && char.IsLetter(c1.Value) => HasKeyword()
                    ? ScannerState.Keyword
                    : ScannerState.Id,

                var (c1, _) when c1.HasValue && CharacterUtilities.IsDigitLetterOrUnderscore(c1.Value) => ScannerState
                    .Id,

                _ => ScannerState.Nothing
            };

            if (State == ScannerState.Nothing) return null;
            NextNode();
            return CurrentNode;
        }

        [return: NotNull]
        public IEnumerable<SyntaxToken> ScanTillEnd()
        {
            var node = Scan();
            var tokens = new LinkedList<SyntaxNode>();

            while (node != null)
            {
                tokens.AddLast(node);
                node = Scan();
            }

            if (SafePeekChar() != null) throw GetException($"No token matched for {CurrentChar}.");
            return tokens.MergeTriviaToTokens();
        }

        private ScannerException GetException([DisallowNull] string message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            return new ScannerException(CurrentCharOffset, message);
        }

        private char PeekP1() => Source[CurrentCharOffset + 1];

        private void NextNode()
        {
            CurrentNode = State switch
            {
                ScannerState.Keyword => ScanKeyword(),
                ScannerState.Number => ScanNumber(),
                ScannerState.String => ScanString(),
                ScannerState.Id => ScanId(),
                ScannerState.BlockComment => ScanBlockComment(),
                ScannerState.LineComment => ScanLineComment(),
                ScannerState.Html => ScanHtml(),
                ScannerState.Preprocessor => ScanPreprocessor(),
                ScannerState.Whitespace => ScanWhitespace(),
                ScannerState.Punctuation1 => ScanPunctuation1(),
                ScannerState.Punctuation2 => ScanPunctuation2(),
                ScannerState.Nothing => throw GetException("Invalid parser state."),
                _ => throw new InvalidOperationException()
            };
        }

        [return: NotNull]
        private SyntaxNode ScanPunctuation1()
        {
            var c = CurrentChar;
            var start = CurrentCharOffset;
            NextChar();
            var k = Grammar.Punctuation.GetValueOrDefault($"{c}");
            if (k == SyntaxKind.Nothing) throw GetException($"Expecting punctuation, found {CurrentChar}.");
            return new PunctuationSyntax(k, start, 1, $"{c}");
        }

        [return: NotNull]
        private SyntaxNode ScanPunctuation2()
        {
            var start = CurrentCharOffset;
            var c1 = CurrentChar;
            NextChar();
            var c2 = CurrentChar;
            NextChar();
            var k = Grammar.Punctuation.GetValueOrDefault($"{c1}{c2}");
            if (k == SyntaxKind.Nothing) throw GetException($"Expecting punctuation, found {CurrentChar}.");
            return new PunctuationSyntax(k, start, start + 2, $"{c1}{c2}");
        }

        [return: NotNull]
        private SyntaxToken ScanHtml()
        {
            Scratch.Clear();
            var start = CurrentCharOffset;
            var level = 1;
            if (CurrentChar != '<') throw GetException($"Expecting <, found {CurrentChar}.");
            Scratch.Append('<');
            NextChar();

            while (true)
            {
                if (Empty()) throw GetException("Unexpected end of file.");
                Scratch.Append(CurrentChar);

                switch (CurrentChar)
                {
                    case '<':
                        level++;
                        break;
                    case '>':
                        level--;
                        break;
                }

                NextChar();
                if (level == 0) break;
                if (level < 0) throw GetException($"Invalid HTML sequence, unexpected {CurrentChar}.");
            }

            return new StringSyntax(
                SyntaxKind.HtmlStringToken,
                start,
                CurrentCharOffset - start,
                Scratch.ToString().Substring(1, Scratch.Length - 2));
        }

        private bool Empty() => !(CurrentCharOffset < Source.Length);

        [return: NotNull]
        private SyntaxToken ScanKeyword()
        {
            Scratch.Clear();
            var start = CurrentCharOffset;
            if (!char.IsLetter(CurrentChar)) throw GetException($"Expecting letter, found {CurrentChar}.");
            Scratch.Append(CurrentChar);
            SyntaxKind k;

            while (true)
            {
                NextChar();
                if (Empty() || Scratch.Length > Grammar.MaxKeywordLength)
                    throw GetException(
                        $"Unexpected end of file or it is impossible to find matching keyword for {Scratch}.");

                var c = CurrentChar;

                if (char.IsLetterOrDigit(c))
                {
                    Scratch.Append(c);
                    k = Grammar.Keywords.GetValueOrDefault(Scratch.ToString());

                    if (k != SyntaxKind.Nothing)
                        break;
                }
                else throw GetException($"Expecting letter or digit, found {CurrentChar}.");
            }

            NextChar();
            return new KeywordSyntax(k, start, CurrentCharOffset - start, Scratch.ToString());
        }

        [return: NotNull]
        private SyntaxTrivia ScanBlockComment()
        {
            var start = CurrentCharOffset;
            Scratch.Clear();
            if (CurrentChar != '/') throw GetException($"Expecting /, found {CurrentChar}.");
            NextChar();
            if (CurrentChar != '*') throw GetException($"Expecting *, found {CurrentChar}.");
            var hasAsterisk = false;

            while (true)
            {
                if (Empty()) throw GetException($"Unexpected end of file.");
                NextChar();
                var c = CurrentChar;

                if (c == '*' && !hasAsterisk)
                    hasAsterisk = true;
                else if (c == '/' && hasAsterisk)
                {
                    NextChar();
                    break;
                }
                else
                {
                    if (hasAsterisk)
                        Scratch.Append('*');

                    hasAsterisk = false;
                    Scratch.Append(c);
                }
            }

            var scanBlockComment = new SyntaxTrivia(
                SyntaxKind.BlockCommentTrivia,
                start,
                CurrentCharOffset - start,
                Scratch.ToString());

            return scanBlockComment;
        }

        [return: NotNull]
        private SyntaxTrivia ScanWhitespace()
        {
            var start = CurrentCharOffset;
            Scratch.Clear();

            while (true)
            {
                if (Empty()) break;
                var c = CurrentChar;
                if (!CharacterUtilities.IsWhitespace(c)) break;
                Scratch.Append(c);
                NextChar();
            }

            return new SyntaxTrivia(SyntaxKind.WhitespaceTrivia, start, CurrentCharOffset - start, Scratch.ToString());
        }

        [return: NotNull]
        private SyntaxTrivia ScanPreprocessor()
        {
            Scratch.Clear();
            var start = CurrentCharOffset;
            if (CurrentChar != '#') throw GetException($"Expecting #, found {CurrentChar}.");

            while (!Empty())
            {
                NextChar();
                var c = CurrentChar;
                if (c == '\n') break;
                Scratch.Append(c);
            }

            return new SyntaxTrivia(SyntaxKind.PreprocessorTrivia, start, CurrentCharOffset - start,
                Scratch.ToString());
        }

        [return: NotNull]
        private SyntaxTrivia ScanLineComment()
        {
            var start = CurrentCharOffset;
            Scratch.Clear();
            if (CurrentChar != '/') throw GetException($"Expecting /, found {CurrentChar}.");
            NextChar();
            if (CurrentChar != '/') throw GetException($"Expecting /, found {CurrentChar}.");

            while (!Empty())
            {
                NextChar();
                var c = CurrentChar;
                if (c == '\n') break;
                Scratch.Append(c);
            }

            return new SyntaxTrivia(SyntaxKind.LineCommentTrivia, start, CurrentCharOffset - start, Scratch.ToString());
        }

        [return: NotNull]
        private SyntaxToken ScanId()
        {
            Scratch.Clear();
            var start = CurrentCharOffset;

            if (!char.IsLetter(CurrentChar) && CurrentChar != '_')
                throw GetException($"Expecting letter or _, found {CurrentChar}.");

            Scratch.Append(CurrentChar);
            NextChar();

            while (true)
            {
                if (Empty()) break;
                var c = CurrentChar;

                if (CharacterUtilities.IsDigitLetterOrUnderscore(c)) Scratch.Append(c);
                else break;

                NextChar();
            }

            return new StringSyntax(SyntaxKind.IdToken, start, CurrentCharOffset - start, Scratch.ToString());
        }

        [return: NotNull]
        private SyntaxToken ScanString()
        {
            var start = CurrentCharOffset;
            Scratch.Clear();
            if (CurrentChar != '"') throw GetException($"Expecting \", found {CurrentChar}.");
            var guarded = false;
            NextChar();

            while (true)
            {
                if (Empty()) throw GetException("Unexpected end of file.");
                var c = CurrentChar;

                switch (c)
                {
                    case '\\' when !guarded:
                        guarded = true;
                        NextChar();
                        continue;
                    case '"' when guarded:
                        Scratch.Append('"');
                        guarded = false;
                        NextChar();
                        continue;
                    case var c1 when c1 != '"' && guarded:
                        Scratch.Append('\\');
                        Scratch.Append(c);
                        guarded = false;
                        NextChar();
                        continue;
                    case var c1 when c1 != '"' && !guarded:
                        Scratch.Append(c);
                        NextChar();
                        continue;
                }

                NextChar();
                if (c != '"' || guarded) continue;
                break;
            }

            return new StringSyntax(SyntaxKind.StringToken, start, CurrentCharOffset - start, Scratch.ToString());
        }

        [return: NotNull]
        private SyntaxToken ScanNumber()
        {
            var sig = "";
            var start = CurrentCharOffset;
            if (Empty()) throw GetException("Unexpected end of file.");

            if (CurrentChar == '-')
            {
                sig = "-";
                NextChar();
            }

            if (CurrentChar == '.')
            {
                NextChar();
                return new StringSyntax(SyntaxKind.NumberToken, start, CurrentCharOffset - start, $"{sig}.{Digits()}");
            }

            var integerPart = Digits();

            if (Empty() || CurrentChar != '.')
                return new StringSyntax(SyntaxKind.NumberToken, start, CurrentCharOffset - start,
                    $"{sig}{integerPart}");

            NextChar();

            return new StringSyntax(
                SyntaxKind.NumberToken,
                start,
                CurrentCharOffset - start,
                $"{sig}{integerPart}.{Digits()}");
        }

        [return: NotNull]
        private string Digits()
        {
            Scratch.Clear();

            while (!Empty() && char.IsDigit(CurrentChar))
            {
                Scratch.Append(CurrentChar);
                NextChar();
            }

            return Scratch.ToString();
        }

        private bool HasKeyword()
        {
            return Grammar.Keywords.Keys.Any(k =>
            {
                if (k == null) throw new ArgumentNullException(nameof(k));
                var p1 = Source.ElementAtOrDefault(CurrentCharOffset + k.Length);

                return !CharacterUtilities.IsDigitLetterOrUnderscore(p1) &&
                       string.Equals(PeekMany(k.Length), k, StringComparison.InvariantCultureIgnoreCase);
            });
        }

        [return: MaybeNull]
        private string PeekMany(int quantity) => Source.Length < quantity + CurrentCharOffset
            ? null
            : Source.Substring(CurrentCharOffset, quantity);

        [return: MaybeNull]
        private char? SafePeekChar()
        {
            if (Empty()) return null;
            return CurrentChar;
        }

        [return: MaybeNull]
        private char? SafePeekCharP1()
        {
            if (!(CurrentCharOffset + 1 < Source.Length)) return null;
            return PeekP1();
        }

        private void NextChar() => CurrentCharOffset++;

        [return: NotNull]
        public override string ToString() => $"{nameof(Source)}: {Source}";

        private enum ScannerState : byte
        {
            Nothing = 0,
            Keyword = 1,
            Number = 2,
            String = 3,
            Id = 4,
            BlockComment = 5,
            LineComment = 6,
            Preprocessor = 7,
            Whitespace = 8,
            Html = 9,
            Punctuation2 = 10,
            Punctuation1 = 11
        }
    }
}