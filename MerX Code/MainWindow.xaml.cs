using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace MerX_Code
{
    public partial class MainWindow : Window
    {
        private Lexer _lexer;
        private Parser _parser;
        private HighlightingColorizer _highlightingColorizer;

        public MainWindow()
        {
            InitializeComponent();

            
            _lexer = new Lexer("");
            _parser = new Parser(new List<Token>());

            // AvalonEdit'in TextChanged olayına abone ol
            Editor.TextChanged += Editor_TextChanged;

            // Kendi renklendirme stratejimizi AvalonEdit'e uygula
            _highlightingColorizer = new HighlightingColorizer(Editor.Document, _lexer);
            Editor.TextArea.TextView.LineTransformers.Add(_highlightingColorizer);

            ApplyTheme(ThemeManager.CurrentTheme);
        }

        private void Editor_TextChanged(object? sender, EventArgs e)
        {
            _lexer = new Lexer(Editor.Text);
            List<Token> tokens = _lexer.Tokenize();
            _highlightingColorizer.SetTokens(tokens);
            _parser = new Parser(tokens); // Parser'ı da güncelleyebiliriz
            _parser.Parse(); // Sözdizimi geçerliliğini kontrol et
            
            Editor.TextArea.TextView.Redraw(); // Burası önemli!
        }

        private void ApplyTheme(ThemeManager.Theme theme)
        {
            ThemeManager.CurrentTheme = theme;
            Editor.Background = ThemeManager.CurrentTheme.Background;
            Editor.Foreground = ThemeManager.CurrentTheme.Foreground;
            Editor.TextArea.TextView.Redraw(); // Tema değiştiğinde renklendirmeyi yeniden çiz
        }

        private void ApplyLightTheme_Click(object sender, RoutedEventArgs e)
        {
            ApplyTheme(ThemeManager.LightTheme);
        }

        private void ApplyDarkTheme_Click(object sender, RoutedEventArgs e)
        {
            ApplyTheme(ThemeManager.DarkTheme);
        }
    }

    // AvalonEdit için özel renklendirme sınıfı
    public class HighlightingColorizer : DocumentColorizingTransformer
    {
        private Lexer _lexer;
        private readonly TextDocument _document;
        private List<Token> _tokens;

        public HighlightingColorizer(TextDocument document, Lexer lexer)
        {
            _lexer = lexer;
            _document = document ?? throw new ArgumentNullException(nameof(document));
            _tokens = new List<Token>(); // Başlangıçta boş token listesi
        }

        // Lexer'ı güncellemek için metod
        public void SetLexer(Lexer lexer)
        {
            _lexer = lexer;
        }

        public void SetTokens(List<Token> tokens)
        {
            _tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            if (_tokens == null || !_tokens.Any())
                return; // Token yoksa bir şey yapma

            int lineStartOffset = line.Offset;
            int lineEndOffset = line.EndOffset;

            foreach (var token in _tokens)
            {
                // Token bu satırın içinde veya bu satırla kesişiyorsa
                if (token.Start < lineEndOffset && token.Start + token.Length > lineStartOffset)
                {
                    // Token'ın satır içindeki başlangıç ve bitiş ofsetlerini hesapla
                    int startOffset = Math.Max(lineStartOffset, token.Start);
                    int endOffset = Math.Min(lineEndOffset, token.Start + token.Length);

                    if (startOffset < endOffset)
                    {
                        // Bu kısımda 'CurrentDocument' yerine 'this.CurrentDocument' veya '_document' kullanabiliriz.
                        // DocumentColorizingTransformer'dan gelen CurrentDocument'ı kullanmak daha doğrudur.
                        ChangeLinePart(startOffset, endOffset, element =>
                        {
                            if (ThemeManager.CurrentTheme.Colors.TryGetValue(token.Type, out Brush? brush) && brush != null)
                            {
                                element.TextRunProperties.SetForegroundBrush(brush);
                            }
                            else
                            {
                                element.TextRunProperties.SetForegroundBrush(ThemeManager.CurrentTheme.Foreground);
                            }
                        });
                    }
                }
            }
        }
    }
}