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
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        [NotNull] private readonly StringBuilder _scratch = new StringBuilder();
        [NotNull] private string _source;
        [MaybeNull] private SyntaxNode? _currentNode;
        private int _currentCharOffset;
        private ScannerState _state = ScannerState.Nothing;

        private char CurrentChar => _source[_currentCharOffset];

        [return: MaybeNull]
        private SyntaxNode Scan()
        {
            _state = (SafePeekChar(), SafePeekCharP1()) switch
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

            if (_state == ScannerState.Nothing) return null;
            NextNode();
            return _currentNode;
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
            return new ScannerException(_currentCharOffset, message);
        }

        private char PeekP1() => _source[_currentCharOffset + 1];

        private void NextNode()
        {
            _currentNode = _state switch
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
            var start = _currentCharOffset;
            NextChar();
            var k = Grammar.Punctuation.GetValueOrDefault($"{c}");
            if (k == SyntaxKind.Nothing) throw GetException($"Expecting punctuation, found {CurrentChar}.");
            return new PunctuationSyntax(k, start, 1, $"{c}");
        }

        [return: NotNull]
        private SyntaxNode ScanPunctuation2()
        {
            var start = _currentCharOffset;
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
            _scratch.Clear();
            var start = _currentCharOffset;
            var level = 1;
            if (CurrentChar != '<') throw GetException($"Expecting <, found {CurrentChar}.");
            _scratch.Append('<');
            NextChar();

            while (true)
            {
                if (Empty()) throw GetException("Unexpected end of file.");
                _scratch.Append(CurrentChar);

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
                _currentCharOffset - start,
                _scratch.ToString().Substring(1, _scratch.Length - 2));
        }

        private bool Empty() => !(_currentCharOffset < _source.Length);

        [return: NotNull]
        private SyntaxToken ScanKeyword()
        {
            _scratch.Clear();
            var start = _currentCharOffset;
            if (!char.IsLetter(CurrentChar)) throw GetException($"Expecting letter, found {CurrentChar}.");
            _scratch.Append(CurrentChar);
            SyntaxKind k;

            while (true)
            {
                NextChar();
                if (Empty() || _scratch.Length > Grammar.MaxKeywordLength)
                    throw GetException(
                        $"Unexpected end of file or it is impossible to find matching keyword for {_scratch}.");

                var c = CurrentChar;

                if (char.IsLetterOrDigit(c))
                {
                    _scratch.Append(c);
                    k = Grammar.Keywords.GetValueOrDefault(_scratch.ToString());

                    if (k != SyntaxKind.Nothing)
                        break;
                }
                else throw GetException($"Expecting letter or digit, found {CurrentChar}.");
            }

            NextChar();
            return new KeywordSyntax(k, start, _currentCharOffset - start, _scratch.ToString());
        }

        [return: NotNull]
        private SyntaxTrivia ScanBlockComment()
        {
            var start = _currentCharOffset;
            _scratch.Clear();
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
                        _scratch.Append('*');

                    hasAsterisk = false;
                    _scratch.Append(c);
                }
            }

            var scanBlockComment = new SyntaxTrivia(
                SyntaxKind.BlockCommentTrivia,
                start,
                _currentCharOffset - start,
                _scratch.ToString());

            return scanBlockComment;
        }

        [return: NotNull]
        private SyntaxTrivia ScanWhitespace()
        {
            var start = _currentCharOffset;
            _scratch.Clear();

            while (true)
            {
                if (Empty()) break;
                var c = CurrentChar;
                if (!CharacterUtilities.IsWhitespace(c)) break;
                _scratch.Append(c);
                NextChar();
            }

            return new SyntaxTrivia(SyntaxKind.WhitespaceTrivia, start, _currentCharOffset - start, _scratch.ToString());
        }

        [return: NotNull]
        private SyntaxTrivia ScanPreprocessor()
        {
            _scratch.Clear();
            var start = _currentCharOffset;
            if (CurrentChar != '#') throw GetException($"Expecting #, found {CurrentChar}.");

            while (!Empty())
            {
                NextChar();
                var c = CurrentChar;
                if (c == '\n') break;
                _scratch.Append(c);
            }

            return new SyntaxTrivia(SyntaxKind.PreprocessorTrivia, start, _currentCharOffset - start,
                _scratch.ToString());
        }

        [return: NotNull]
        private SyntaxTrivia ScanLineComment()
        {
            var start = _currentCharOffset;
            _scratch.Clear();
            if (CurrentChar != '/') throw GetException($"Expecting /, found {CurrentChar}.");
            NextChar();
            if (CurrentChar != '/') throw GetException($"Expecting /, found {CurrentChar}.");

            while (!Empty())
            {
                NextChar();
                var c = CurrentChar;
                if (c == '\n') break;
                _scratch.Append(c);
            }

            return new SyntaxTrivia(SyntaxKind.LineCommentTrivia, start, _currentCharOffset - start,
                _scratch.ToString());
        }

        [return: NotNull]
        private SyntaxToken ScanId()
        {
            _scratch.Clear();
            var start = _currentCharOffset;

            if (!char.IsLetter(CurrentChar) && CurrentChar != '_')
                throw GetException($"Expecting letter or _, found {CurrentChar}.");

            _scratch.Append(CurrentChar);
            NextChar();

            while (true)
            {
                if (Empty()) break;
                var c = CurrentChar;

                if (CharacterUtilities.IsDigitLetterOrUnderscore(c)) _scratch.Append(c);
                else break;

                NextChar();
            }

            return new StringSyntax(SyntaxKind.IdToken, start, _currentCharOffset - start, _scratch.ToString());
        }

        [return: NotNull]
        private SyntaxToken ScanString()
        {
            var start = _currentCharOffset;
            _scratch.Clear();
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
                        _scratch.Append('"');
                        guarded = false;
                        NextChar();
                        continue;
                    case var c1 when c1 != '"' && guarded:
                        _scratch.Append('\\');
                        _scratch.Append(c);
                        guarded = false;
                        NextChar();
                        continue;
                    case var c1 when c1 != '"' && !guarded:
                        _scratch.Append(c);
                        NextChar();
                        continue;
                }

                NextChar();
                if (c != '"' || guarded) continue;
                break;
            }

            return new StringSyntax(SyntaxKind.StringToken, start, _currentCharOffset - start, _scratch.ToString());
        }

        [return: NotNull]
        private SyntaxToken ScanNumber()
        {
            var sig = "";
            var start = _currentCharOffset;
            if (Empty()) throw GetException("Unexpected end of file.");

            if (CurrentChar == '-')
            {
                sig = "-";
                NextChar();
            }

            if (CurrentChar == '.')
            {
                NextChar();
                return new StringSyntax(SyntaxKind.NumberToken, start, _currentCharOffset - start, $"{sig}.{Digits()}");
            }

            var integerPart = Digits();

            if (Empty() || CurrentChar != '.')
                return new StringSyntax(SyntaxKind.NumberToken, start, _currentCharOffset - start,
                    $"{sig}{integerPart}");

            NextChar();

            return new StringSyntax(
                SyntaxKind.NumberToken,
                start,
                _currentCharOffset - start,
                $"{sig}{integerPart}.{Digits()}");
        }

        [return: NotNull]
        private string Digits()
        {
            _scratch.Clear();

            while (!Empty() && char.IsDigit(CurrentChar))
            {
                _scratch.Append(CurrentChar);
                NextChar();
            }

            return _scratch.ToString();
        }

        private bool HasKeyword()
        {
            return Grammar.Keywords.Keys.Any(k =>
            {
                if (k == null) throw new ArgumentNullException(nameof(k));
                var p1 = _source.ElementAtOrDefault(_currentCharOffset + k.Length);

                return !CharacterUtilities.IsDigitLetterOrUnderscore(p1) &&
                       string.Equals(PeekMany(k.Length), k, StringComparison.InvariantCultureIgnoreCase);
            });
        }

        [return: MaybeNull]
        private string PeekMany(int quantity) => _source.Length < quantity + _currentCharOffset
            ? null
            : _source.Substring(_currentCharOffset, quantity);

        [return: MaybeNull]
        private char? SafePeekChar()
        {
            if (Empty()) return null;
            return CurrentChar;
        }

        [return: MaybeNull]
        private char? SafePeekCharP1()
        {
            if (!(_currentCharOffset + 1 < _source.Length)) return null;
            return PeekP1();
        }

        private void NextChar() => _currentCharOffset++;

        [return: NotNull]
        public override string ToString() => $"{nameof(_source)}: {_source}";

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