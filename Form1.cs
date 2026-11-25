using System;
using System.Drawing;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NTP_SpechApp
{
    public partial class Form1 : Form
    {
        // Arayüz elemanlarý (TTS - Metinden Sese)
        private TextBox txtMetin = null!;
        private ComboBox cmbSesler = null!;
        private TrackBar trkHiz = null!;
        private TrackBar trkSesDuzeyi = null!;
        private Label lblHizDeger = null!;
        private Label lblSesDeger = null!;
        private Button btnOku = null!;
        private Button btnDuraklat = null!;
        private Button btnDurdur = null!;
        private Button btnWavKaydet = null!;

        // Arayüz elemanlarý (STT - Sesten Metne)
        private Button btnDinleBaslat = null!;
        private Button btnDinleDurdur = null!;
        private Button btnTemizle = null!;
        private Label lblDurum = null!;

        // Konuþma motorlarý
        private SpeechSynthesizer? sentezleyici;
        private SpeechRecognitionEngine? tanimaMotoru;
        private bool dinleniyorMu = false;

        public Form1()
        {
            InitializeComponent();
            ArayuzuKur();
            KonusmaMotorlariniHazirla();
        }

        private void ArayuzuKur()
        {
            this.Text = "YMH219 - SpeechApp (TTS & STT)";
            this.ClientSize = new Size(560, 470);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormClosing += Form1_FormClosing;

            // Metin Giriþ Alaný
            Label lblMetin = new Label() { Text = "Okunacak / Tanýnan Metin:", Location = new Point(15, 15), AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold) };
            txtMetin = new TextBox()
            {
                Location = new Point(15, 38),
                Size = new Size(530, 145),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Segoe UI", 10.5f),
                Text = "Merhaba! Bu uygulama C# Windows Forms ve System.Speech kütüphanesi kullanýlarak geliþtirilmiþtir."
            };

            // Ses Seçimi
            Label lblSesSec = new Label() { Text = "Ses / Dil:", Location = new Point(15, 198), AutoSize = true };
            cmbSesler = new ComboBox() { Location = new Point(85, 195), Size = new Size(460, 25), DropDownStyle = ComboBoxStyle.DropDownList };

            // Hýz Ayarý
            Label lblHiz = new Label() { Text = "Okuma Hýzý:", Location = new Point(15, 238), AutoSize = true };
            trkHiz = new TrackBar() { Location = new Point(105, 232), Size = new Size(140, 35), Minimum = -10, Maximum = 10, Value = 0, TickFrequency = 2 };
            lblHizDeger = new Label() { Text = "0", Location = new Point(250, 238), AutoSize = true };
            trkHiz.Scroll += (s, e) => lblHizDeger.Text = trkHiz.Value.ToString();

            // Ses Düzeyi Ayarý
            Label lblSes = new Label() { Text = "Ses Düzeyi:", Location = new Point(290, 238), AutoSize = true };
            trkSesDuzeyi = new TrackBar() { Location = new Point(370, 232), Size = new Size(135, 35), Minimum = 0, Maximum = 100, Value = 100, TickFrequency = 10 };
            lblSesDeger = new Label() { Text = "100%", Location = new Point(510, 238), AutoSize = true };
            trkSesDuzeyi.Scroll += (s, e) => lblSesDeger.Text = trkSesDuzeyi.Value + "%";

            // TTS Butonlarý
            GroupBox grpTts = new GroupBox() { Text = "Metinden Sese (Text-to-Speech)", Location = new Point(15, 278), Size = new Size(530, 68) };

            btnOku = new Button() { Text = "Seslendir", Location = new Point(12, 24), Size = new Size(115, 32) };
            btnOku.Click += BtnOku_Click;

            btnDuraklat = new Button() { Text = "Duraklat", Location = new Point(137, 24), Size = new Size(115, 32) };
            btnDuraklat.Click += BtnDuraklat_Click;

            btnDurdur = new Button() { Text = "Durdur", Location = new Point(262, 24), Size = new Size(115, 32) };
            btnDurdur.Click += BtnDurdur_Click;

            btnWavKaydet = new Button() { Text = "WAV Kaydet", Location = new Point(387, 24), Size = new Size(130, 32) };
            btnWavKaydet.Click += BtnWavKaydet_Click;

            grpTts.Controls.AddRange(new Control[] { btnOku, btnDuraklat, btnDurdur, btnWavKaydet });

            // STT Butonlarý
            GroupBox grpStt = new GroupBox() { Text = "Sesten Metne (Speech-to-Text - Mikrofon)", Location = new Point(15, 355), Size = new Size(530, 68) };

            btnDinleBaslat = new Button() { Text = "Dinlemeyi Baþlat", Location = new Point(12, 24), Size = new Size(165, 32) };
            btnDinleBaslat.Click += BtnDinleBaslat_Click;

            btnDinleDurdur = new Button() { Text = "Dinlemeyi Durdur", Location = new Point(187, 24), Size = new Size(165, 32), Enabled = false };
            btnDinleDurdur.Click += BtnDinleDurdur_Click;

            btnTemizle = new Button() { Text = "Metni Temizle", Location = new Point(362, 24), Size = new Size(155, 32) };
            btnTemizle.Click += (s, e) => txtMetin.Clear();

            grpStt.Controls.AddRange(new Control[] { btnDinleBaslat, btnDinleDurdur, btnTemizle });

            // Durum Etiketi
            lblDurum = new Label() { Text = "Durum: Hazýr", Location = new Point(15, 435), ForeColor = Color.DarkBlue, AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold) };

            this.Controls.AddRange(new Control[]
            {
                lblMetin, txtMetin, lblSesSec, cmbSesler,
                lblHiz, trkHiz, lblHizDeger, lblSes, trkSesDuzeyi, lblSesDeger,
                grpTts, grpStt, lblDurum
            });
        }

        private void KonusmaMotorlariniHazirla()
        {
            // TTS Hazýrlýðý
            try
            {
                sentezleyici = new SpeechSynthesizer();
                sentezleyici.SetOutputToDefaultAudioDevice();
                sentezleyici.SpeakCompleted += (s, e) => DurumGuncelle("Durum: Seslendirme tamamlandý.", Color.DarkGreen);

                foreach (InstalledVoice ses in sentezleyici.GetInstalledVoices())
                {
                    if (ses.Enabled)
                    {
                        cmbSesler.Items.Add(ses.VoiceInfo.Name);
                    }
                }

                if (cmbSesler.Items.Count > 0)
                    cmbSesler.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                DurumGuncelle("TTS Hatasý: " + ex.Message, Color.Red);
            }
        }

        // METNÝ SESLENDÝR
        private void BtnOku_Click(object? sender, EventArgs e)
        {
            if (sentezleyici == null) return;
            string metin = txtMetin.Text.Trim();
            if (string.IsNullOrEmpty(metin))
            {
                MessageBox.Show("Lütfen seslendirilecek bir metin girin.", "Uyarý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                sentezleyici.SpeakAsyncCancelAll();
                if (cmbSesler.SelectedItem != null)
                    sentezleyici.SelectVoice(cmbSesler.SelectedItem.ToString()!);

                sentezleyici.Rate = trkHiz.Value;
                sentezleyici.Volume = trkSesDuzeyi.Value;
                btnDuraklat.Text = "Duraklat";

                DurumGuncelle("Durum: Metin seslendiriliyor...", Color.DarkOrange);
                sentezleyici.SpeakAsync(metin);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Seslendirme hatasý: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // DURAKLAT / DEVAM ET
        private void BtnDuraklat_Click(object? sender, EventArgs e)
        {
            if (sentezleyici == null) return;

            if (sentezleyici.State == SynthesizerState.Speaking)
            {
                sentezleyici.Pause();
                btnDuraklat.Text = "Devam Et";
                DurumGuncelle("Durum: Seslendirme duraklatýldý.", Color.DarkOrange);
            }
            else if (sentezleyici.State == SynthesizerState.Paused)
            {
                sentezleyici.Resume();
                btnDuraklat.Text = "Duraklat";
                DurumGuncelle("Durum: Seslendirme devam ediyor...", Color.DarkOrange);
            }
        }

        // DURDUR
        private void BtnDurdur_Click(object? sender, EventArgs e)
        {
            if (sentezleyici == null) return;
            sentezleyici.SpeakAsyncCancelAll();
            btnDuraklat.Text = "Duraklat";
            DurumGuncelle("Durum: Seslendirme durduruldu.", Color.DarkBlue);
        }

        // WAV DOSYASINA KAYDET
        private async void BtnWavKaydet_Click(object? sender, EventArgs e)
        {
            string metin = txtMetin.Text.Trim();
            if (string.IsNullOrEmpty(metin))
            {
                MessageBox.Show("Önce kaydedilecek metni girin.", "Uyarý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = "WAV Ses Dosyasý (*.wav)|*.wav",
                FileName = "KonusmaKaydi.wav"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string seciliSes = cmbSesler.SelectedItem?.ToString() ?? "";
                int hiz = trkHiz.Value;
                int sesDuzeyi = trkSesDuzeyi.Value;
                string dosyaYolu = sfd.FileName;

                try
                {
                    DurumGuncelle("Durum: WAV dosyasý oluþturuluyor...", Color.DarkOrange);
                    await Task.Run(() =>
                    {
                        using SpeechSynthesizer dosyaSentezleyici = new SpeechSynthesizer();
                        if (!string.IsNullOrEmpty(seciliSes))
                            dosyaSentezleyici.SelectVoice(seciliSes);

                        dosyaSentezleyici.Rate = hiz;
                        dosyaSentezleyici.Volume = sesDuzeyi;
                        dosyaSentezleyici.SetOutputToWaveFile(dosyaYolu);
                        dosyaSentezleyici.Speak(metin);
                    });

                    DurumGuncelle("Durum: WAV dosyasý baþarýyla kaydedildi!", Color.DarkGreen);
                    MessageBox.Show("Ses dosyasý kaydedildi:\n" + dosyaYolu, "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    DurumGuncelle("Durum: Kayýt hatasý.", Color.Red);
                    MessageBox.Show("WAV kaydetme hatasý: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // SESTEN METNE (STT) BAÞLAT
        private void BtnDinleBaslat_Click(object? sender, EventArgs e)
        {
            try
            {
                if (tanimaMotoru == null)
                {
                    var taninicilar = SpeechRecognitionEngine.InstalledRecognizers();
                    if (taninicilar.Count == 0)
                    {
                        MessageBox.Show("Sistemde yüklü konuþma tanýma (Speech Recognition) paketi bulunamadý.\nWindows Ayarlar > Zaman ve Dil > Konuþma bölümünden dil paketi eklenebilir.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    tanimaMotoru = new SpeechRecognitionEngine(taninicilar[0]);
                    tanimaMotoru.LoadGrammar(new DictationGrammar());
                    tanimaMotoru.SpeechRecognized += (s, args) =>
                    {
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() => txtMetin.AppendText(" " + args.Result.Text)));
                        }
                        else
                        {
                            txtMetin.AppendText(" " + args.Result.Text);
                        }
                    };
                    tanimaMotoru.SetInputToDefaultAudioDevice();
                }

                tanimaMotoru.RecognizeAsync(RecognizeMode.Multiple);
                dinleniyorMu = true;
                btnDinleBaslat.Enabled = false;
                btnDinleDurdur.Enabled = true;
                DurumGuncelle("Durum: Mikrofon dinleniyor...", Color.DarkGreen);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mikrofon veya konuþma tanýma baþlatýlamadý:\n" + ex.Message, "Uyarý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DurumGuncelle("Durum: Mikrofon baþlatýlamadý.", Color.Red);
            }
        }

        // SESTEN METNE (STT) DURDUR
        private void BtnDinleDurdur_Click(object? sender, EventArgs e)
        {
            if (tanimaMotoru != null && dinleniyorMu)
            {
                tanimaMotoru.RecognizeAsyncStop();
                dinleniyorMu = false;
                btnDinleBaslat.Enabled = true;
                btnDinleDurdur.Enabled = false;
                DurumGuncelle("Durum: Dinleme durduruldu.", Color.DarkBlue);
            }
        }

        private void DurumGuncelle(string mesaj, Color renk)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, Color>(DurumGuncelle), mesaj, renk);
                return;
            }
            lblDurum.Text = mesaj;
            lblDurum.ForeColor = renk;
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            sentezleyici?.SpeakAsyncCancelAll();
            sentezleyici?.Dispose();
            if (tanimaMotoru != null)
            {
                if (dinleniyorMu) tanimaMotoru.RecognizeAsyncCancel();
                tanimaMotoru.Dispose();
            }
        }
    }
}
