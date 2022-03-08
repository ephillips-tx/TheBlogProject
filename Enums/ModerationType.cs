using System.ComponentModel;

namespace TheBlogProject.Enums
{
    public enum ModerationType //limits reasons that comment was modified. Avoids the "I just don't like it" reason
    {
        //can also use annotation on an enumeration
        [Description("Political propaganda")]
        Political,
        [Description("Offensive Language")]
        Language,
        [Description("Drug References")]
        Drugs,
        [Description("Threatening Speech")]
        Threatening,
        [Description("Sexual Content")]
        Sexual,
        [Description("Hate Speech ")]
        HateSpeech,
        [Description("Targeted Shaming")]
        Shaming
    }
}
