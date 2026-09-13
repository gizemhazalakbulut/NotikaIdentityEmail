namespace NotikaIdentityEmail.Entities
{
    public class Message
    {
        public int MessageId { get; set; }
        public string SenderEmail { get; set; } // Mesajı gönderen kullanıcının e-posta adresi
        public string ReceiverEmail { get; set; } // Mesajı alan kullanıcının e-posta adresi
        public string Subject { get; set; } // Mesajın konusu
        public DateTime SendDate { get; set; } // Mesajın gönderildiği tarih
        public string MessageDetail { get; set; } // Mesajın içeriği
        public bool IsRead { get; set; } // Mesajın okunup okunmadığını belirten bir bayrak
        public int CategoryId { get; set; } // Mesajın ait olduğu kategoriye referans
        public Category Category { get; set; } // Mesajın ait olduğu kategoriye ait nesne
    }
}
