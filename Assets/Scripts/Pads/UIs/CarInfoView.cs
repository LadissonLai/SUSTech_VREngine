using Doozy.Engine.UI;
using Framework;
using Fxb.CPTTS;
using TMPro;
using UnityEngine.U2D;
using UnityEngine.UI;
using Fxb.CMSVR;

public class CarInfoView : PadViewBase
{
    public TextMeshProUGUI carName;

    public TextMeshProUGUI framework;

    public Image carIcon;

    public TextMeshProUGUI date;

    public TextMeshProUGUI miles;

    public TextMeshProUGUI color;

    public TextMeshProUGUI infoTitle;

    public TextMeshProUGUI information;

    public SpriteAtlas spriteAtlas;

    protected override void Start()
    {
        base.Start();

        var data = World.current.Injecter.Get<SoftwareCsvConfig>().GetRowDatas(0);

        carName.text = data.Channel;

        framework.text = $"车架号：{data.Framework}";

        carIcon.sprite = spriteAtlas.GetSprite(data.Icon);

        date.text = data.ProductDate;

        miles.text = data.Miles;

        color.text = data.Color;

        infoTitle.text = data.InfoTitle;

        information.text = data.Information;
    }
}
