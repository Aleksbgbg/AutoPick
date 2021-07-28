namespace AutoPick.Views
{
    using AutoPick.Converters;

    public partial class StateView
    {
        public StateView(InfoTextConverter infoTextConverter, InfoIconConverter infoIconConverter)
        {
            Resources.Add("InfoTextConverter", infoTextConverter);
            Resources.Add("InfoIconConverter", infoIconConverter);

            InitializeComponent();
        }
    }
}