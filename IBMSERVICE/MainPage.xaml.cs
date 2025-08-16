using Microsoft.CognitiveServices.Speech;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.ML.Transforms.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IBMSERVICE
{
    public partial class MainPage : ContentPage
    {
        private string userText;
        private string systemText;
        private string inputprompt;
        private string outputkonteks;
        private string RESPONSEAI;
        private string RESPONSEDUA;
        private string accessToken;
        private Editor AIBOT;
        private string UNDOTEKS;
        private string? REDOTEKS;
        private string LOKASIUSER;
        private SpeechRecognizer speechRecognizer;
        public class Messages
        {
        public string Role { get; set; }
        public object Content { get; set; }  // bisa List<ContentItem> atau string
        }
        public class Body
        {
        public List<Messages> Messages { get; set; }
        public string Project_Id { get; set; }
        public string Model_Id { get; set; }
        public int Frequency_Penalty { get; set; }
        public int Max_Tokens { get; set; }
        public int Presence_Penalty { get; set; }
        public int Temperature { get; set; }
        public int Top_P { get; set; }
         }
         public class MessageContent
        {
            public string type { get; set; }
            public string text { get; set; }
        }
        public class Message
        {
            public string role { get; set; }
            public List<MessageContent> content { get; set; }
        }
        public class Payload
        {
            public List<Message> messages { get; set; }
            public string project_id { get; set; }
            public string model_id { get; set; }
            public int frequency_penalty { get; set; }
            public int max_tokens { get; set; }
            public int presence_penalty { get; set; }
            public int temperature { get; set; }
            public int top_p { get; set; }
            public object seed { get; set; }
            public List<object> stop { get; set; }
        }
        public MainPage()
        {
            InitializeComponent();
        }
        private async Task<string?> GetAccessTokenAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://iam.cloud.ibm.com/identity/token");

                request.Content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("grant_type", "urn:ibm:params:oauth:grant-type:apikey"),
                new KeyValuePair<string, string>("apikey", "vjVGUbjhZHU6mYwjnRvwQNo0ZvpRO9N2dQoigvA2OAH3")
            });

                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                var response = await client.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Ambil access_token dari JSON
                    using (var doc = JsonDocument.Parse(responseContent))
                    {
                        var token = doc.RootElement.GetProperty("access_token").GetString();
                        return token;
                    }
                }
                else
                {
                    throw new Exception($"Gagal ambil token: {response.StatusCode}\n{responseContent}");
                }
            }
        }
        private async void ASSISTANTNEED()
        {
            try
            {
                Microsoft.CognitiveServices.Speech.SpeechConfig BOT = SpeechConfig.FromEndpoint(new Uri("https://westus2.api.cognitive.microsoft.com/"));
                speechRecognizer = new SpeechRecognizer(BOT);
                await speechRecognizer.StartContinuousRecognitionAsync();
                string input = speechRecognizer.RecognizeOnceAsync().Result.Text;
                switch (input)
                {
                    case "Hi":
                        await PROSSESORIBM("");
                        break;
                    case "Uang":
                        await PROSSESORIBM("");
                        break;
                    case "Kembalian":
                        await PROSSESORIBM("");
                        break;
                    case "Pinjam":
                        await PROSSESORIBM("");
                        break;
                    case "Hello":
                        await PROSSESORIBM("");
                        break;
                    case "Saya Mau Beli":
                        await PROSSESORIBM("");
                        break;
                    case "Apakah Kamu Punya Uang":
                        await PROSSESORIBM("");
                        break;
                    case "A":
                        await PROSSESORIBM("");
                        break;
                }
            }
            finally
            {
                await speechRecognizer.StopContinuousRecognitionAsync();
            }
        }
        private async Task PROSSESORIBM(string input)
        {
            switch (input)
            {
                case string A when A == "" && A.Contains(""):
                    input += A + "";
                    break;
                case string B when B == "" && B.Contains(""):
                    input += B + "";
                    break;
                case string C when C == "" && C.Contains(""):
                    input += C + "";
                    break;
                case string D when D == "" && D.Contains(""):
                    input += D + "";
                    break;
            }
            string url = "https://us-south.ml.cloud.ibm.com/ml/v1/text/chat?version=2023-05-29";
                var payload = new Payload
                {
                    messages = new List<Message>
                {
                new Message
                {
                    role = "user",
                    content = new List<MessageContent>
                    {
                        new MessageContent
                        {
                            type = "text",
                            text = input,
                        },
                    }
                }
                },
                    project_id = "e47e2d70-d100-4517-88bb-373082e5b84d",
                    model_id = "ibm/granite-3-3-8b-instruct",
                    frequency_penalty = 0,
                    max_tokens = 2000,
                    presence_penalty = 0,
                    temperature = 0,
                    top_p = 1,
                    seed = null,
                    stop = new List<object>()
                };
                string jsonPayload = JsonConvert.SerializeObject(payload, Formatting.Indented);
                using (HttpClient client = new HttpClient())
                {
                    accessToken = await GetAccessTokenAsync();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                    try
                    {
                        HttpResponseMessage response = await client.PostAsync(url, content);
                        string result = await response.Content.ReadAsStringAsync();
                        if (response.IsSuccessStatusCode)
                        {
                            JObject obj = JObject.Parse(result);
                            string assistantMessage = (string)obj["choices"][0]["message"]["content"];
                            await TextToSpeech.Default.SpeakAsync(assistantMessage, new SpeechOptions()
                            {
                                Volume = 100,
                                Pitch = 1,
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception occurred:");
                        Console.WriteLine(ex.Message);
                    }
                finally
                {
                }
                }
        }
        private async Task AIKASSISTANT()
        {
            string input = INPUTMESIN.Text;
            foreach (var item in INPUTMESIN.Text.Split(" "))
            {
                await TextToSpeech.Default.SpeakAsync(item);
            }
            switch (input)
            {
                //AUTO UNTUK INDUSTRI
                case string A when A.Contains(""):
                    break;
                case string B when B.Contains(""):
                    break;
                case string C when C.Contains(""):
                    break;
                case string D when D.Contains("") && D.StartsWith(""):
                    break;
            }
        }
        private async void BISINDOEMOJIFITUR()
        {
            List<string> DATAGAMBARBISINDO = new List<string>()
            {

            };
            List<string> EMOJIBISINDO = new List<string>()
            {
                "🤞","🖐️","👇","a.jpg",
            };
            List<ImageButton> KEYBOARD = new List<ImageButton>()
            {
            };
            string[] KALIMATBUILDER = new string[24];
            speechRecognizer.StartContinuousRecognitionAsync().RunSynchronously();
            string pairing = speechRecognizer.RecognizeOnceAsync().Result.Text;
            var bites = Encoding.UTF8.GetBytes(pairing);
            for (int i = 0; i < bites.Length; i++)
            {
                
            }
            List<string> OLAHTEKS = pairing.Split(" ").ToList();
            for (int i = 0; i < OLAHTEKS.Count; i++)
            {
                for (global::System.Int32 j = 0; j < EMOJIBISINDO.Count; j++)
                {
                    switch (OLAHTEKS[i])
                    {
                        case string A when OLAHTEKS[i] == "A" && EMOJIBISINDO[j] == "":
                            A = OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            KALIMATBUILDER[0] = A;
                            break;
                        case string B when OLAHTEKS[i] == "B" && EMOJIBISINDO[j] == "":
                            B = OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            KALIMATBUILDER[1] = B;
                            break;
                        case string C when OLAHTEKS[i] == "C" && EMOJIBISINDO[j] == "":
                            C = OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            KALIMATBUILDER[2] = C;
                            break;
                        case string D when OLAHTEKS[i] == "D" && EMOJIBISINDO[j] == "":
                            D = OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            KALIMATBUILDER[3] = D;
                            break;
                        case string E when OLAHTEKS[i] == "E" && EMOJIBISINDO[j] == "":
                            E = OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            KALIMATBUILDER[4] = E;
                            break;
                        case string F when OLAHTEKS[i] == "F" && EMOJIBISINDO[j] == "":
                            F = OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            KALIMATBUILDER[5] = F;
                            break;
                        case string G when OLAHTEKS[i] == "G" && EMOJIBISINDO[j] == "":
                            G = OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            KALIMATBUILDER[6] = G;
                            break;
                        case string H when OLAHTEKS[i] == "H" && EMOJIBISINDO[j] == "":
                            H = OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            KALIMATBUILDER[7] = H;
                            break;
                        case string I when OLAHTEKS[i] == "I" && EMOJIBISINDO[j] == "":
                            I = OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            KALIMATBUILDER[8] = I;
                            break;
                        case string J when OLAHTEKS[i] == "J" && EMOJIBISINDO[j] == "":
                            J = OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            KALIMATBUILDER[9] = J;
                            break;
                        case string K when OLAHTEKS[i] == "K" && EMOJIBISINDO[j] == "":
                            K = OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            KALIMATBUILDER[10] = K;
                            //APA UNTUK IMAGINE CUP JUGA YA INI
                            break;
                        case string L when OLAHTEKS[i] == "L" && EMOJIBISINDO[j] == "":
                            L = OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            KALIMATBUILDER[11] = L;
                            break;
                        case string M when OLAHTEKS[i] == "M" && EMOJIBISINDO[j] == "":
                            M = OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            KALIMATBUILDER[12] = M;
                            break;
                        case string N when OLAHTEKS[i] == "N" && EMOJIBISINDO[j] == "":
                            N = OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            KALIMATBUILDER[13] = N;
                            break;
                        case string O when OLAHTEKS[i] == "O" && EMOJIBISINDO[j] == "":
                            O = OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            KALIMATBUILDER[14] = O;
                            break;
                        case string P when OLAHTEKS[i] == "P" && EMOJIBISINDO[j] == "":
                            P = OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            KALIMATBUILDER[15] = P;
                            break;
                        case string Q when OLAHTEKS[i] == "Q" && EMOJIBISINDO[j] == "":
                            Q = OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            KALIMATBUILDER[16] = Q;
                            break;
                        case string R when OLAHTEKS[i] == "R" && EMOJIBISINDO[j] == "":
                            R = OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            KALIMATBUILDER[17] = R;
                            break;
                        case string S when OLAHTEKS[i] == "S" && EMOJIBISINDO[j] == "":
                            S = OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            KALIMATBUILDER[18] = S;
                            break;
                        case string T when OLAHTEKS[i] == "T" && EMOJIBISINDO[j] == "":
                            T = OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            KALIMATBUILDER[19] = T;
                            break;
                        case string U when OLAHTEKS[i] == "U" && EMOJIBISINDO[j] == "":
                            U = OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            KALIMATBUILDER[20] = U;
                            break;
                        case string V when OLAHTEKS[i] == "V" && EMOJIBISINDO[j] == "":
                            OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            break;
                        case string W when OLAHTEKS[i] == "W" && EMOJIBISINDO[j] == "":
                            W = OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            KALIMATBUILDER[21] = W;
                            break;
                        case string X when OLAHTEKS[i] == "X" && EMOJIBISINDO[j] == "":
                            X = OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            KALIMATBUILDER[22] = X;
                            break;
                        case string Y when OLAHTEKS[i] == "Y" && EMOJIBISINDO[j] == "":
                            Y = OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            KALIMATBUILDER[23] = Y;
                            break;
                        case string Z when OLAHTEKS[i] == "Z" && EMOJIBISINDO[j] == "":
                            Z = OLAHTEKS[i].Replace(OLAHTEKS[i], EMOJIBISINDO[j]);
                            KALIMATBUILDER[24] = Z;
                            break;
                    }
                }
                for(int oke = 0; oke < KALIMATBUILDER.Count(); oke++)
                {
                    ImageButton KEYBOARD = new ImageButton();
                    KEYBOARD.HeightRequest = 150;
                    KEYBOARD.WidthRequest = 150;
                    KEYBOARD.CornerRadius = 20;
                    KEYBOARD.BorderColor = Colors.Cyan;
                    KEYBOARD.BorderWidth = 5;
                    KEYBOARD.Margin = 5;
                    switch(KALIMATBUILDER[oke])
                    {
                        case "Sayang":
                            KEYBOARD.Source = KALIMATBUILDER[oke];
                            KEYBOARD.Command = new Command(async () =>
                            {
                                await PROSSESORIBM(KALIMATBUILDER[oke]);
                            });
                            break;
                        default:
                            break;
                    }
                    KEYBOARDDISABILITAS.Children.Insert(0, KEYBOARD);
                }
                //GO TO MIT
                //pasti bisa pelukan pertama di sg
                //SUDAH ADA 3 PEREMPUAN CINTA TULUS
                //CALON PENERUS ALPHABET 
                //PASTI PUNYA PACAR
            }
        }
        private async void TranskriptTool()
        {

            try
            {
                await speechRecognizer.StartContinuousRecognitionAsync();
                var input = speechRecognizer.RecognizeOnceAsync();
                string message = input.Result.Text;
                if (!string.IsNullOrEmpty(message))
                {
                    RoundRectangle roundRectangle = new Microsoft.Maui.Controls.Shapes.RoundRectangle()
                    {
                        CornerRadius = new CornerRadius(20, 10, 20, 10),
                        Stroke = new LinearGradientPaint()
                        {
                            StartColor = Colors.Black,
                            EndColor = Colors.Purple,
                            StartPoint = new Point(10, 0),
                            EndPoint = new Point(20, 10),
                        },
                        StrokeThickness = 10,
                    };
                    //tambahkan fitur aplikasi catatan pribadi lalu tambahkan tombol grounding data catatan pribadi ke dalam bot gpt
                    //tambahkan fitur pencarian frasa otomatis menggunakan seleksi teks dan button dengan keluaran output layout menggunakan bottom sheet dan editor jadi bisa diatur user untuk
                    AIBOT = new Editor
                    {
                        Text = message.Replace("##", "").Replace("**", "").Replace("#", "").Replace("*", ""),
                        BackgroundColor = Colors.LightCyan,  // Transparent background for the bubble effect
                        TextColor = Colors.Black,
                        Margin = new Thickness(0),
                        CharacterSpacing = 3,
                        IsReadOnly = true,
                        FontAutoScalingEnabled = true,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        IsSpellCheckEnabled = true,
                        IsTextPredictionEnabled = true,
                    };
                    var messageFrame = new Border
                    {
                        StrokeShape = roundRectangle,
                        Padding = new Thickness(0),
                        Margin = new Thickness(1),
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        Shadow = new Shadow
                        {
                            Brush = Colors.LightCyan,
                            Opacity = 0.2f,
                            Offset = new Point(2, 2),
                            Radius = 5
                        },
                        Content = new StackLayout
                        {
                            Orientation = StackOrientation.Vertical,  // Label inside the bubble
                            Padding = new Thickness(1),
                            Children =
                    {
                       AIBOT,
                    }
                        }
                    };
                    TapGestureRecognizer EDITOR = new TapGestureRecognizer()
                    {
                        NumberOfTapsRequired = 2,
                    };
                    EDITOR.Tapped += (S, E) =>
                    {
                        AIBOT.IsReadOnly = false;
                    };
                    messageFrame.Background = new LinearGradientPaint()
                    {
                        StartColor = Colors.White,
                        EndColor = Colors.LightGrey,
                        StartPoint = new Point(2, 0),
                        EndPoint = new Point(1, 0)
                    };
                    //tambahkan fitur seleksi pesan + tombol kerja
                    var list = new CollectionView()
                    {
                        Header = messageFrame,
                        ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepLastItemInView,
                        FlowDirection = FlowDirection.MatchParent,
                        CanReorderItems = true,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        ItemsLayout = LinearItemsLayout.Vertical,
                        VerticalScrollBarVisibility = ScrollBarVisibility.Default,
                    };
                    TRANSKRIPNOTE.Children.Insert(0, list);
                }
            }
            finally
            {
                await speechRecognizer.StopContinuousRecognitionAsync();
            }
        }
        //TAMBAHKAN FITUR KONVE
        private async Task<string> ENTERPRISEDATAINTERNAL(string message)
        {
            throw new NotImplementedException();
        }
        private async Task SPEAKBOT()
        {
            try
            {
                TextToSpeech.Default.SpeakAsync(AIBOT.Text).Wait(AIBOT.Text.Length);
            }
            finally
            {
            }
        }
        private void SERVICE_Clicked(object sender, EventArgs e)
        {

        }
        private void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
        {

        }
        private void SwipeGestureRecognizer_Swiped_1(object sender, SwipedEventArgs e)
        {

        }

        private void SlotView_ImagesLoaded(System.Object sender, System.EventArgs e)
        {

        }

        private void SlotView_StartInteraction(System.Object sender, Microsoft.Maui.Controls.TouchEventArgs e)
        {

        }

        private void SlotView_StartHoverInteraction(System.Object sender, Microsoft.Maui.Controls.TouchEventArgs e)
        {

        }

        private void SlotView_Started(System.Object sender, System.EventArgs e)
        {

        }

        private void SlotView_EndInteraction(System.Object sender, Microsoft.Maui.Controls.TouchEventArgs e)
        {

        }

        private void SlotView_EndHoverInteraction(System.Object sender, System.EventArgs e)
        {

        }
    }
}
