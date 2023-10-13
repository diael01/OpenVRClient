using System.Windows.Media;

namespace RaceClient.Models
{
  public class Assists
  {
    public int IDEAL_LINE { get; set; }         //0,1 on or off
    public ImageSource Ideal { get; set; }
    public int AUTO_BLIP { get; set; }          //0,1 on or off
    public ImageSource Blip { get; set; }
    public int STABILITY_CONTROL { get; set; }  //0-100 % of stability control increments of 1
    public string Stability { get; set; }
    public int AUTO_BRAKE { get; set; }         //0,1 on or off
    public ImageSource Brake { get; set; }
    public int AUTO_SHIFTER { get; set; }       //0,1 on or off
    public ImageSource Shifter { get; set; }
    public int ABS { get; set; }                //0,1,2 Off On or Factory
    public ImageSource Abs { get; set; }
    public int TRACTION_CONTROL { get; set; }   //0,1,2 Off On or Factory
    public ImageSource Traction { get; set; }
    public int AUTO_CLUTCH { get; set; }        //0,1 on or off
    public ImageSource Clutch { get; set; }
    public int VISUALDAMAGE { get; set; }       //0,1 on or off
    public ImageSource Visual { get; set; }
    public int DAMAGE { get; set; }             //0-100 increments of 10
    public int FUEL_RATE { get; set; }          //0.00-5.00 increments .0.01 
    public int TYRE_WEAR { get; set; }          //0.00-5.00 increments .0.01 
    public int TYRE_BLANKETS { get; set; }      //0,1 on or off
    public ImageSource Tyre { get; set; }
    public int SLIPSTREAM { get; set; }         //1-10

    public ImageSource AssistImage { get; set; }

    public ImageSource CarImage { get; set; }

    public ImageSource TrackImage { get; set; }

    public ImageSource VariationImage { get; set; }

    public ImageSource BrandImage { get; set; }

    public ImageSource CountryImage { get; set; }
  }

  public class ImagesPath {
    public string CarImage { get; set; }

    public string TrackImage { get; set; }

    public string VariationImage { get; set; }

    public string BrandImage { get; set; }

    public string CountryImage { get; set; }

  }


}
