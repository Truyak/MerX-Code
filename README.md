# MerX Code

MerX Code, C# ve WPF ile geliştirilmiş bir gerçek zamanlı sözdizimi vurgulayıcıdır. Kod yazarken belirteç türlerini anında farklı renklerle vurgular ve açık/koyu tema seçenekleri sunar. AvalonEdit metin editörü kullanılır, ancak vurgulama mantığı tamamen özgün kodlarla yazılmıştır.

## Özellikler
- Gerçek zamanlı sözdizimi vurgulama (anahtar kelimeler, değişkenler, sayılar, dizeler, operatörler, yorumlar).
- Açık ve koyu tema desteği.
- Kullanıcı dostu WPF arayüzü.

## Daha Fazla Bilgi
Projenin detaylı açıklaması ve geliştirme süreci için Medium yazımı okuyabilirsiniz:  
[MerX Code: Gerçek Zamanlı Sözdizimi Vurgulayıcı](https://medium.com/@kerem.ozcan2004/merx-code-kendi-ger%C3%A7ek-zamanl%C4%B1-s%C3%B6zdizimi-vurgulay%C4%B1c%C4%B1n%C4%B1z%C4%B1-kendiniz-geli%C5%9Ftirin-f32325238338)

## Tanıtım Videosu
Uygulamanın özelliklerini görmek için tanıtım videosunu izleyebilirsiniz:  
[Tanıtım Videosu]([YouTube-videonuzun-bağlantısı](https://youtu.be/6miwVdFR8wI))

## Kurulum ve Çalıştırma
1. Repoyu klonlayın:  
   ```
   git clone https://github.com/kullanıcıadın/MerX-Code.git
   ```
2. Visual Studio’da `MerX-Code.sln` dosyasını açın.
3. Gerekli bağımlılıkları yükleyin (AvalonEdit için NuGet paketi: `Install-Package ICSharpCode.AvalonEdit`).
4. Projeyi derleyin ve çalıştırın.

## Dosyalar
- `App.xaml` & `App.xaml.cs`: Uygulamanın başlangıç noktası.
- `MainWindow.xaml` & `MainWindow.xaml.cs`: Arayüz ve olay yönetimi.
- `SyntaxHighlighter.cs`: Leksikal analiz, sözdizimi analizi ve tema yönetimi.
- `img/logo.ico`: Arayüz simgesi.
