# YMH219-SpeechApp

Kısa Açıklama: Windows Forms ve `System.Speech` kütüphanesi kullanılarak geliştirilmiş, metinden sese (Text-to-Speech), WAV ses dosyasına kayıt ve sesten metne (Speech-to-Text) özelliklerine sahip masaüstü uygulaması.

## Kullanılan Teknolojiler

- C# / .NET Windows Forms
- System.Speech (NuGet Paketi - `SpeechSynthesizer` & `SpeechRecognitionEngine`)

## Kurulum ve Çalıştırma Adımları

1. Bu depoyu yerel bilgisayarınıza klonlayın:
   `git clone https://github.com/250542008-sanat/YMH219-SpeechApp.git`
2. Proje klasörüne girin ve `NTP_SpechApp.csproj` dosyasını Visual Studio ile açın.
3. NuGet paketlerinin (`.NET` sürümüne uyumlu `System.Speech`) geri yüklendiğinden emin olun.
4. Üst menüden **Başlat (Start)** butonuna basarak veya `F5` tuşu ile uygulamayı çalıştırın.
5. Metin kutusuna okunacak metni yazın, ses/hız/ses düzeyi ayarlarını seçip **Seslendir** veya **WAV Kaydet** butonunu kullanın.

## Uygulamanın Temel Özellikleri

- **Metinden Sese (Text-to-Speech):** Sistemde yüklü ses motorlarını (ör. Microsoft Zira Desktop) listeleyerek metni asenkron olarak seslendirme.
- **Ses Kontrolü:** Okuma hızını (`-10` ile `+10` arası) ve ses düzeyini (`0%` - `100%`) anlık ayarlayabilme; seslendirmeyi duraklatma, devam ettirme ve durdurma.
- **WAV Dosyasına Aktarma:** Girilen metni arka planda (arayüzü kilitlemeden) `.wav` ses dosyası olarak bilgisayara kaydedebilme.
- **Sesten Metne (Speech-to-Text):** Windows konuşma tanıma motoru ve mikrofon üzerinden konuşulanları metin alanına aktarma.

## Ekran Görüntüleri

<img width="1470" height="956" alt="Ekran Resmi 2026-11-25 15 45 49" src="https://github.com/user-attachments/assets/a20c08f3-36ca-4064-afd1-8bdd907421d1" />
<img width="1470" height="956" alt="Ekran Resmi 2026-11-25 15 45 59" src="https://github.com/user-attachments/assets/c79205f8-008b-42dd-95ee-5992ea8e3918" />


## Bilinen Sınırlamalar / Geliştirme Notları

- `System.Speech` kütüphanesi yalnızca Windows işletim sistemlerinde desteklenmektedir.
- Sesten metne (Speech-to-Text) özelliğinin çalışabilmesi için sistemde aktif bir mikrofon ve Windows konuşma tanıma dil paketinin yüklü olması gerekir.
- Seslendirme kalitesi ve dil desteği, bilgisayarda kurulu olan Windows TTS ses paketlerine bağlıdır.

## Geliştirici

Ad-Soyad: Mustafa Şahingöz  
Öğrenci Numarası: 250542008
