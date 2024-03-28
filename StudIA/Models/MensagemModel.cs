using System;
using System.Windows.Media.Imaging;

namespace StudIA.Models
{
    public class MensagemModel
    {
        public string Username { get; set; }
        public BitmapImage ImageSource { get; set; }
        public string Mensagem { get; set; }
        public DateTime Time { get; set; }
        public bool IsNativeOrigin { get; set; }
        public bool? FirstMessage { get; set; }
    }
}
