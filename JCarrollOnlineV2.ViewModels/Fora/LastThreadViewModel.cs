using JCarrollOnlineV2.ViewModels.Users;
using System;

namespace JCarrollOnlineV2.ViewModels.Fora
{
    public class LastThreadViewModel : ForaViewModelBase
    {
        public DateTime UpdatedAt { get; set; }
        public string Title { get; set; }
        public int PostRoot { get; set; }
        public int PostNumber { get; set; }
        public ApplicationUserViewModel Author { get; set; }
        public ForaViewModel Forum { get; set; }
    }

}
