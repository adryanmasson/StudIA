using StudIA.Models;
using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StudIA
{
    public class SessionService
    {
        public static UserModel UsuarioAtual { get; set; }
        public static string caminhoImagemPadrao = "../../../Images/fotodeperfil.jpg";
        public static byte[] bytesImagemPadrao = File.ReadAllBytes(caminhoImagemPadrao);

        public static string caminhoImagemPadraoIA = "../../../Images/ftdeperfilEDU.png";
        public static byte[] bytesImagemPadraoIA = File.ReadAllBytes(caminhoImagemPadraoIA);

        public static BitmapImage ImagemPadraoIA = ConverterBytesParaBitmapImage(bytesImagemPadraoIA);


        public static BitmapImage ConverterBytesParaBitmapImage(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return ConverterBytesParaBitmapImage(SessionService.bytesImagemPadrao);
            }

            BitmapImage imagem = new BitmapImage();

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                stream.Position = 0;

                imagem.BeginInit();
                imagem.CacheOption = BitmapCacheOption.OnLoad;
                imagem.StreamSource = stream;
                imagem.EndInit();
            }

            return imagem;
        }
    }
}
