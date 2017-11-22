using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WEP.GSP.Document;

namespace WEP.GSP.ServiceCall
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //string myEventHubMessage = "{ \"RequestToken\": \"aasdfgdfga\", \"RequestType\":1, \"Clientid\":\"l7xxdf2b47b7d728426699a05c8d1ec33a60\", \"Statecd\":\"11\", \"Username\":\"WeP\", \"Txn\":\"returns\", \"ClientSecret\":\"30a28162eb024f6e859a12bbb9c31725\", \"IpUsr\":\"12.8.9l.80\", \"Blob\":1, \"BlobFile\":\"l7xxdf2b47b7d728426699a05c8d1ec33a60_WeP_11_12.8.9l.80_636286450812343303\", \"AuthToken\":\"8a227e0ba56042a0acdf98b3477d2c03\" }";
                string myEventHubMessage = "{\"action\":\"RETSAVE\",\"data\":\"BVNGJWRiRY+Drw+PKvadKhdt4/6l3aW5beXgZT605xxC+qH3OTPIO9Bar0BvtTW46/BdB2RIrqxdbymdorbs7bcaVIfGlaH3DDKVORPJHctEJo/BpKLowEpeKHD6QOw9/vQCzLxX0L06Rep0xbUexK1P3TOcP1uquHw7BJ9/8qlVdi69XnKmUsXtAkz7Mf0ljG9ZlLlBcIIHOLxni9sk2GioLhsv+ju/CDLENkpl9P6O0P+WcNfZZiC9u1Rbrc9nnxYssdASjtdEgI05fI9uRMSIZsJG+BsS0bh7NvIg5iCsjnaKdsS/vD3WUJ2eKLswf4C+x8eijIqEMXN9P2TP3ua/HkAnAGJbcsAsERPUaH0UJrE2+6Q5RFZvNOz0JND2y6teu58PGz3BB81zOOWz9KatHY9fxBWDWeKiswPeJ9IoEAk1IkYsEyX+HoSkA1U0c0abMN+0c4YyUPKoJiW7ErWPxvyONRsw2IFU1W1ECaP/ZQeAbGiPYFvGEJQ6fdR1D7gnCxvWKpeCLaVIKHIDE/AstKbHnEzDFZAv8EFXEv5cMNX5dfFDVNJz+2DJT2hK5sArX7615W7G0q38UZVxmIxvWZS5QXCCBzi8Z4vbJNihT0v44nHkcX297UxityHSEkfMGuAnqHwH/aGwz1jAOvk6C82PZ2p33Gj2FtLzlbvk6F226WaxRvHb2C+R7XxWdf0HDlFw8X1hZNw6ZLhYfgljZxxwhy/pnXoGs5ss7o3db5J8yz4BEIV3ZdFiJP4x5OhdtulmsUbx29gvke18VhBUZjK/VMV0ZGy0WxeuiT/QpZBPH2ZSbSff0mSJ3xfIAx6G074sI5FrWM0/4NJnQL8LX96M3m30WX3VfXoSUoKZvjakeDAspbukm2PMNSFGbH7/2gm77spb53VBYXs24vl9a8m+bTzRqRcvUUvMQISa6BSJLwnIT9PSwLUzxR2MeV7v+tqVEoSecyqCFAX/V/81p3fGtXOrxmQ/wniNNf/QpZBPH2ZSbSff0mSJ3xfIrqMZw3943C8dFyYF4MAacyEKav8fqjQ+Yyn+Rnw7DqKiDz/bUv/izI9W0teODE+chpOpw42aBXyn3RBOvomO+cXv3VtmBRZR9/GM8IhsYS8LVNoaJkoOBcf5hCzn9ZBxL34fFBFBpO1Vovj7SmMGFpPbe53GW41GIhuktVqxsrYW9TgLRmrGhe7jq7B8WcD1H/Vjuj7WYvK8NUTUY+Ch9f16eehzeZb9/ES6l+rWqpk5OpWg0fmzo2oT7+kJWpLzEGwFTwMoyz/yLVBGQQqHBfwbpAPh9QyWJ+2dbYPx6DQMZJMXACs+MtsTUG/2tjyvX82dxffr7FrWllNWxpnRgSETeRPK3eouzRcPA58u8gnXdCXSA18JPMrSbo74jb2R1xKHPR41jWoo/PLrX2uA4++BTEkvdy+mRgDg+HyF24TeXw3WaT3qymFcsrfZxhq8o/3IxrNlReDybGGKfLcLRbCpz0z+2zYNv3gy3b0ars4MA2rtstv10d2Ow4RfbXN+KoZaRKM6fVad4U291IBz5+HIo7mH9HDr2atFwk7ivfocmkk8EJBNwlbiP8qCftl+Qc6hOjInNYrCIKANqo5NJMoeLjD1eAALACZh03FH+665uwvsK0qy8lwEozA3zqwLQVuzmBSYYD/HY3iMgyBHn5T35E7capFHccUIhS48uU+BVEgqjjAvhjs92QzAYpYv+ZUFHP+fzuyqyuXNBw4vle3B/PpZ+b8nSp/qHHNPHznMQgtdW0LCCIIuK3/QTZRvDPAo1B5wKTeTVjkcsC6iJAxkkxcAKz4y2xNQb/a2PK9Vdsxb38oYHoIIgKVetdv0gC+FKC7UiCkBOkHUCu4QhiH3LNjboTa7yqipOjb4haBRXLCydk59AZO0YN/F/UryfXK/EYdcDOMu6mFp+tzx71IKS+o+5N5uAKGlEAGPDwZORCH2tWNAFuCTTj6ATTOlspiroYYsYfLTYxAF3BK8542PsFUiQCQR1WdJyssm7MU5eT8ZA0lR7pA/SJ/yhmwImyQUWLCxOhsL8hSdBiLh4tekdx3K5lft/vLR0e5lXAThL9Zx5Ouj7wEjLrewNQQ2L+dmB4dU84pklW6G/Ac2yLnQFpWxInQc1OrH1/XWJDhBERkwQfY7hvdNL38hjKN9diVB/s5Q5/01rf3gzLDpHeN1nT0Xe/crf5VLCFKh0me1jy7Zybw1zqt7t2CaGx+PG6PvOgl1CP5esCYYUwDFsGsFxjTxINvZAbkuyd7fx5V0hBHxY+jwSNK4yujlNb3K7EBaBugwt6G6hanq7Kjxy3dTTVeHZ/tQb8HNkrCRWZbK5s5OEBy53Jnru3olTNkAyNTT2RhwORnJ9vNerIDjUY908EcItSuhppwYnmXZ2Ymg/5Eg/tMlKESHTfS699WHIQtQmZ1AwtmxflwsCWiouldQN73bBpUWKxoJurlgqeFpkzSRDXoUfMSgI8sUKTzBVpVC1IW/gohujVc31A1q4Gsj2VSUPIQ9xPXxqGd0JC9uWzRfpcS7wR9p2oRHNdlqT+Rdth95vHqqUukg378gK2/ZxBqBzcq47qsSm/reJPPwbkbRa+3i9JBz7Lid8HYr0VhMTQ40f2yUD7i7fLkSO+eMcYFiHPpIAOdBInxMuCTXEoc9HjWNaij88utfa4DjlvP4045ab3T5ZpCesUk08SqGWkSjOn1WneFNvdSAc+fhyKO5h/Rw69mrRcJO4r36T3iB+g87A/srpNy+jTVo52+HZnItVJ4+f9fZ7eEjFmB19LXtMbwEM5aQkZ/QbC8Aap5k0z0vFski8+1A5PPYt8OWnjp92iRp6Oh8rWQxG/PyU3drwWB5DQJaHzCcZuIOru+DJlvF6aPDcjN0mGbMCRnOqESZi6Ij/CBij7OmQVQc3UhvC229WOY3d2ji5W0wlhqB/WaFWfH1dR23mzivxAOaEVKaeHWg2zhRmFrJx9Wc2jc3/A6jIhyxzUWd+q9XFhUgPsw6X5Xkb3fq978wVwF6PezD5nNSwKTDb+QDqNhy+n6GOhNVbeP+A5KsbRk25OhdtulmsUbx29gvke18ViMSsVK6IRE01duxVvp13TJ2JUH+zlDn/TWt/eDMsOkdA9EBOBTJd2u+ZPIZ7C35xhJHzBrgJ6h8B/2hsM9YwDpV0OSg1/FYiUjDhm7fYraBgC+FKC7UiCkBOkHUCu4QhmOL94uOc7A2RYiRYZCoJ4Hk6F226WaxRvHb2C+R7XxWll7w+3f7+mZOrZpLptueo5GhY16BtgmvkmmE0JZWm9WNzZIy8aRrsUbPfd2bWl5FDQqAZESOrqTfv427fi7y7gtU2homSg4Fx/mELOf1kHFSQXQaNDHGZ+7C+852EUQwiGK0qyf6AuYKmxCVtM4IinjxDkgqpxKqA4YusG5/jo2klCPaBYAUh/DYU5sfjE1+CeXe0EIgAhw+UX15U4vi0T9ZFkWtLfjyUNKsUGSBw81yQIG6y49QDx+SFRWiOgWSQVvmX8OMZDpqPd6hNUabJ6LIPjT9MVzLZzk3lUeTHDK2Wf1FD/OZl++rOud0vWnspDedEgkq7U5X1MVjw30PzWXU8dexrvBbk2I6k85AYhATs5ttbf85yer9CJn2f1jSt2OHfXa39x61CZmZ+w8M7+sZirl5fSg4VYWi8L3E8/ZtwwKaoQlzYIMcey4YqbKNa2T2HWLdA5HIGRDIhsBQExqahPqzoX0oKH+3nUwjC8i39TT0PLosHRojbGqdMe7QmD7T/xueJR6hQfGdN8tqH7MwvRvlegfajCQEJM2en7nGHkG9IYupCVw2HqAfxDmXKBAJNSJGLBMl/h6EpANVNHNGmzDftHOGMlDyqCYluxK1j8b8jjUbMNiBVNVtRAmj/2UHgGxoj2BbxhCUOn3UdQ+4Jwsb1iqXgi2lSChyAxOqU2IVYrOtOnmxH62UWJF120C21ugRPfuFoST4yQwisubAK1++teVuxtKt/FGVcZiMb1mUuUFwggc4vGeL2yTYoU9L+OJx5HF9ve1MYrch0hJHzBrgJ6h8B/2hsM9YwDr5OgvNj2dqd9xo9hbS85W75OhdtulmsUbx29gvke18VnunJ4YMg72u6oMRwQznyMIJY2cccIcv6Z16BrObLO6NpyjOxaQVKlPnS0d6+Hh+C+ToXbbpZrFG8dvYL5HtfFYQVGYyv1TFdGRstFsXrok/0KWQTx9mUm0n39Jkid8XyAMehtO+LCORa1jNP+DSZ0C/C1/ejN5t9Fl91X16ElKCmb42pHgwLKW7pJtjzDUhRqj4bkbs4Ocevti/06xBCVx7YP79Lvj56kRrAqo+cAFm0BrzvNKwdTKqOuCc4jXT5nxA6h7SgB/BtxgZqAQcxANqHKrVjTcwQitj6MDa9maHyubOThAcudyZ67t6JUzZALt2/dYoDkK+tewHwYp1hw1swVCz7BVq54+QuA1cGomEofnnEF7A4XOhd/TNoLwxtsNVgor+59nPXydmif3ydsTZFWw6tSKbJCzqw/cKWeTc/GG2Ek3au1YnHdfw6HINRVV2f5Jgp4rKVkkFvEiGMma8CS0ecz3H1CUhZ4T2eWneXVGInugTuIM3LqOqb5sww0ZCDG2zwtLYBbTeRgytSxdiA/ZE9+QmkNq6K/J08HThrPhYzqIbob7D5LO1IZBNNcz63nybtuWCci5eFv9ICq3OrdsyXsj0MQLn3rP/rO3uSWJXhadaCKp72GSLf9HrXon/SpB7vzbS+TALoRD+qrLGsNNeFF+IDy1L08OiDyK4K8pKrApY4jj+GuC4Nj/OIX9ISRanRQdFd+vlkukTZwumrR2PX8QVg1niorMD3ifSDO8Ts7Zo5twSAeQgtQyy3jpR3B5KPGdXm3Wj6xQz+m8s/2d7Ppg4dbq1GN4yCmk4zdVYca25jMExezRNL17YIm5DgikieGsyfsbyLvaqF/P78U3/EsqD5PdxEix0ZKvbmVwxqMUXH7JmCEizm9i7Ilvb7SC/rdlUvM9p8p7f91hQrqx12NH5WoIq6tQYfv4t4OGy6HECq1SYje1jeKj0Az8FkR2p7kyTh5W18m8CIc08XHl6ZGnRp/Ui/ZfA1vCI/13ppE0xedeH91XI6Kj92lbKf/5v8aU4QPNaowQubRH/80L3xuPg3RIX6qoBzv81hDa8l2NY1vWHHegT8A6FgoSsah81i6pD9Oqk8nqPjKIuAV/zdPJiFNpGYA1Mzt1cjucOeIH18hE4sWsUNHf+lu/HR1n9dc41RQ4QsezOUmIgqrgfltjUt9XS+zLILzxbAx6G074sI5FrWM0/4NJnQCd1p6TbhuYh9zEIKsUS66B5uZOKTlnQJhgBcFGHRwAr0+nvlqs1hxSu/ehUJlBVAgVsKDFMBXL/zpWxElQ0trCksHaFRaZOIvoiXzsYa12ezcV5nFgIKW2s/rtvDhexMM6FPWvx51NAXq0xQLdDCTMF8sv72nRJBWxp7k4Id6fPShHfnpwTEC68TL7SKrA0/pe4UqiYwsR+BHg+Yiz+3lNVgslL2R+XOZigbwGYPNDcwO0MjxXaOXQSfEVU6OE53cL8pIy1wZ/or22cBqvtoQJ9o59qE3hM7k2EmkvK5+Aovhw6PWrzz/vqQU8R7OVrk0TUAepYMmFOoWLTaNHYu8e7ZVlsr1B8zAut+ae6vmDhjoDQOk1zTLxvUJfW/YN9j5WjGBEh5whopVUTNXP4eqwePtK0A1o2AEccXHwr+JEH95tdkfGvhkJw5mLw6vpdd2rTBb1MhV3NpCeDIzbm6RyS8onnTDov+g2zED1T0eP9T70VczQ7rvdnobe/WGpyg9spHHYsHGMRskiETrr4mKUvijPRgUaqRvOkYQdDJRX/iUpUp67l+bgDuUAO5NBGAsSIZsJG+BsS0bh7NvIg5iBciiKDimfG6AoAVVL9f4043fj+Uahv3/6VXCS+Jj8STM7APThBXvlBF6ULSV8h8+OuFZ1P2qhysfGtH8/20W3dIGF8Ii+cV/cecRgz9f36AcSIZsJG+BsS0bh7NvIg5iDzjUUrDtJEza9WuUVPJrr0GpqE+rOhfSgof7edTCMLyCCquB+W2NS31dL7MsgvPFuebrq1Ah2Ra3QvtyYJH1AmuRPNtXYdVYPoRXIj0xXuppzKWOQc4oQufqx7B+dc7qveOvvXxOi7lRcYfyUpkVezTkuuzEjtYLtDDBJwcmv3iC4vdCwIi+bNGYPqLbOVkvzFadFmT8keB+ysrfd4ti2T0sZfWn+D2pKreBt8L/wdl09JojuFpbeqEteceh9xnL+iDz/bUv/izI9W0teODE+cwxH0b4L2AJODTXWJ6k/XT3mxVcFWNvBtzKrvZQU6vHePrJioalGoq+4Tfrv/OAF/azMpqW0oNAOE/jnByBqXmhj41mNubi7M45cExoWtn2Wt4ERJ76XtkQFS9nrXrWh0UK6sddjR+VqCKurUGH7+LQcCdEvmShlaUOTn2bdvUzsFfinPM2hsEYLWBOAjksMLz9nZvRD4X0rPg8ZcwGoom4xvWZS5QXCCBzi8Z4vbJNjkvn7DjJSpa6Wd8JZKuaYp+F+rqLYOjwpYFJqeVZUDlZsr6AZcYGgqO29v5ixqN+LI/dBIgN5ScxXdiQjZRng/nSKWRwYpVoi2TZF6fcH5SxZVN58yKBscGn7Ny/DMoV8/I0hsqLe86VYpN/Z7FKpXxIhmwkb4GxLRuHs28iDmIIaZYZMVCYvVJj9EbfRCfJD3QaxBKJDhke66OMDOx4/vhpUGwEu8d13rHkkl5X5G7ovKzi+aBuXwM+m5l0qRZTY34wByta2/QYUjQ25Hs2sBxDJ0OvN7Yb+0R65Pp4MuyFwdJeIb98hqa1ZAjNKmyqfqrmkMW57TtIWC5QSvpxMrwJ3Fijx5qXSn4RZjXZ+Z1pGcDk3b6ACzgS8wjX4zeNO4RKbkeY2comheQ/Fq9ChQnm66tQIdkWt0L7cmCR9QJrkTzbV2HVWD6EVyI9MV7qacyljkHOKELn6sewfnXO6r3jr718Tou5UXGH8lKZFXs05LrsxI7WC7QwwScHJr94guL3QsCIvmzRmD6i2zlZL8xWnRZk/JHgfsrK33eLYtk9LGX1p/g9qSq3gbfC/8HZdPSaI7haW3qhLXnHofcZy/og8/21L/4syPVtLXjgxPnMMR9G+C9gCTg011iepP1095sVXBVjbwbcyq72UFOrx3j6yYqGpRqKvuE367/zgBf2szKaltKDQDhP45wcgal5oY+NZjbm4uzOOXBMaFrZ9lYeFjo07trkhOrnnDBziLv8uREPIJ6iwcxK9MnHkVZ4tGSV8//G2L8fJ+zDmUF2Hiw21GQi8G4y/io39syRd/3YhhetAAPZs9YqUM4w9y1E7VNlUtPr5nmEZnMUtIJGsgu2RuOEMtNzA6CKUrvN2fd2ifuXkRAiJB0UlKJT2/6DCZ4k37J3zwPkfYjT+WsddzgKojfOqqCC/n7XaAw3Z0ZM1FIdPQ0Njs7td/lbwUKa/5SaD+SUAx3nHQU/+JHCcWG2+oTzBNjhsA32+hXC/tW960bm8ccMIhex/UHCkA9XWTHEwRYN1Vtq99itLoRb4skpF7peS5UgVeI9ANP8OEMcvVWW09uygvsdKWcC9OAsbWw1I1BgTzLTMz8kWGt54h0CAtJcvPOLYmBGPfljqtOl87EMtOgkhIibYQ+eAp6H6z9wXcf68VyENnsBROy1GG4cuWaFB9aM0l3gU8X1uO19xvZiuUi7rRk0vpjCSsQoFf7OPlbfn1IJu5WTdjFzdWXos6Y04Y1TSQKV8WhmCu8myCBAqKb1SGPc7uAkY6+QN6NbtK5EreN98kLdhRhLERhpOpw42aBXyn3RBOvomO+cXv3VtmBRZR9/GM8IhsYS8Y+NZjbm4uzOOXBMaFrZ9l1hdrAT0OSfQuBBiQmALKq6SUI9oFgBSH8NhTmx+MTX4BtkaowzGUPbM/QrZNJ0LxBRDokZ7BTbp2sSs0/bIn9RBqydmzZtRqj2R2l0urYA+FZ90FYB17J1FUdog3MeAP8niqRE5mS4oWkWf+6bvPgk7rDxupHQaI4U0EBbp3eX718EZdzTsqZ7QE7y2gvZIdNDXvkLhCLxgDPd7qBEUpDYSJQnm/LM7qPUfwSxDr5fWmMHVYu6Lv8J45L7oO1TfT91wwji4AMim+CrtbwMT47aXgaXX4nS40L42Lu93kG7jcAav8uuBiMh4+dd6kFd/IGAQlqNv6shoJlgwdDBek3xKAmtvb7dNWCCveFfhQdZVufrnEVXjL59rW/GLKXuRackCBusuPUA8fkhUVojoFkvbu+P92ltR19kSSsgXbe28+gvU6+kklRwydxIpe+jZWtcUkE4LXZ4m11j/g8zLXvp2CvJuhWpQbMPl3RpXIdvlEPgANf0i/GvJQx1lJI6IfXiBekECLcGgOCTKhGb++9rOuJfxh0PJK6gzesFCNZQ5v5CaLId4dDP+6HcOUnP+gqn9EbW95kC6UVQ5BjwFDmrV+a9zGcvAMzuxA8gVirceQwnDVjLgRPx1UaKy8WriDSWJXhadaCKp72GSLf9HrXqUqCOfwx4MagKPf/TcvoxotVkjnQ9kggWHsjSdLjmZCdmpkUphqGiZ4r6WL69Egy+xAWgboMLehuoWp6uyo8cubUKWInpCG63HCdAnsNkawFvU4C0ZqxoXu46uwfFnA9bpNoKPZRFlPKaBzJtfrjDKAKUVjHuFIdm4I4cpzkVAPKqxD72r8KfNaoAnYnJvpt4CFz/6dHlKdO41rs5JF/4sRfivyUFhxFOQRSi+hFufj2+WM2NFG9s33vP/YoYlc8uZtW2zC00PEiGkxM5tj93qcqQHSHAXP/0LOnkarpJ62xIhmwkb4GxLRuHs28iDmIFGTQbnlvO3PSrk1SBhWF27yYgILBDYQ1URyCgZiqSgqczBrTAe0eUxELtx/gbqLlZTV1arrmD6LnlYhpIFdjf1N+ID2gM8oZxZ/jmh5OoFHb8OC8GdU3o9GhBMWFlT5316cwE6o22iM11GBplgFjvBmME+HMHRwPue0U4ey3BJ4qeZ1jDPGto0u7HLtyPRQ1bj1K8+YuWgeQnupxDvgo/4XcYdFcbB3v0Ob4dF8Tz5TwO0MjxXaOXQSfEVU6OE53cL8pIy1wZ/or22cBqvtoQK6MxQUS0Lvj4yD7+0wvsm55rP1QmbLddnFjusbVxk++nYlQf7OUOf9Na394Myw6R2bqyzm/OZKAb8kQh/NZgddwc4DGiVVtpQ31u52/7IdrL9F/SOdh8dJZ/Yfn7sO1i2ymKuhhixh8tNjEAXcErznjY+wVSJAJBHVZ0nKyybsxSo+1/NaEFrDcos6l7TSDfmuLc/8atBCNaClh/5O2CDBgfSMP8eMMxZqvkHhz1U30pJqfTultWkB6FbxxC1xcXFjSggKkh2yl/tHpPIvmeQYbhqgAxOqbBjHfHNblXMzgZDCcNWMuBE/HVRorLxauIPpnM0dpVhWQFfDmKyGRsHyEkfMGuAnqHwH/aGwz1jAOi3kaNrFAnvME8RppDtz2HlkO+jrgLawWQRpVqWY6/22C8f6QyYc7fdZSx2UtAXoSuxAWgboMLehuoWp6uyo8ct3U01Xh2f7UG/BzZKwkVmWdB9NhCigIkF4r4v6WuCxxZi/eb1IOenTxBMRMI8pMejyasOmNTz/9WBTS29mq/WMjZZU5LLwmpGGo4pzHfBVN8GzlGLWwCA+EBPQzOodZKkuIupYqw54/Rmqu8HXFwVUh3JN4TqjS9gPkiKgc4RujtAxdmM/64tpa1o9q7J/cR6HeTUAAnJ62gaF/y+O7j0DaH99v8yYMYYK9U3b8g1ewUGsv+98WhHrGjtoetMDfonXEoc9HjWNaij88utfa4Dj2HCNsIXQ1Eu3LZTTDhxflsCMARdgixAJ6bMOmmvgyhTs34g+bRbO35l/WLZJw8ZbWrusUp1pOQecq5TE0vhExYIfbt/rrml4Zzs1OhWAyUKWQfOFkOUUas2MW0pY8+sSuGAbDPAoo2mNBXXeK4ce14BRsMVDObGsrQ+HO4QrVaUqHlse1IydeOWTxq5AWvAjeUAWkILfa95CTS0Dav9BPoZB3K/sIWrJc74myu454OFS6RCTtPf8ZL7bg7oyUzQr6ssCNCOiPup1zGEc3kdZakaOJ7M1q7YK6aDbKVQqZ2aO0P+WcNfZZiC9u1Rbrc9nYBcJoCa8fAp32b5qvhUbXeOD6WkVPLR1AzEBbaI864Ac3UhvC229WOY3d2ji5W0wlhqB/WaFWfH1dR23mzivxFFcsLJ2Tn0Bk7Rg38X9SvJ9cr8Rh1wM4y7qYWn63PHv5PySCB/3Ui91Q+L1pilbdsHHXRWwOEI35h4LdZ6vVIJNXjgbVpXQm1UrHt8uA3o5w3/Z2NT1rkEy4ZFOCYO/014gXpBAi3BoDgkyoRm/vvZ2JUH+zlDn/TWt/eDMsOkdZWy0lj8MAaLCG/zsB78niW0NsPk4MYUDPl6+pVnAMe7M+t58m7blgnIuXhb/SAqtXzZTICDj7UPOKZl+m9nk/mP8OZeCykIYiDBWO2gbpMtJYleFp1oIqnvYZIt/0etetY8u2cm8Nc6re7dgmhsfjxuj7zoJdQj+XrAmGFMAxbBG6J/5/CYiUwwRRV0n5bNDq6yZBfjfOlPL0ZC3arVMJjpg6ayGn6wyHYTiyBMQxAqCtmVL7L21OpeDtryCWEsWSEdqe6ITQ+tIvQgXdntbP4JPaYeFY1UsUFGYQvNPGOd7ug/bCNkZ65dFXjB0/Zb6uRApaDySXhExxBPlLrZs6icfxifhi3KoaQmKES4B2/qMb1mUuUFwggc4vGeL2yTYLbchc2td+5whDy/ZPOmuvuJlA85fcfOTcuQ3XLYS/hNsXjfhq6Q6FU9DVJePLw72TMQv87zPGBGwAEusyL0mLl6RvzbMYfJj//g75IByiFSVg/I/lHAKMdVIR7zN9ei43+GBHqvw052SbOnUcxBthC8BnefGficfiyoRHg3F9RNRdGDnTWR6p7UuLlZeejFxZCQ/Kheque/6VK9TH+1S3kBFTqWza1GfeFJlIFoxo6INxgiKutlImR9HsD6tD1eeORyIk+c7xEaT9dsFxfPnW06uMejqCB00e23sgukQJLJHbnnDc/AWe54Iy/+DgAkRoY6XWflsueaDnRm54jBAuy21gjzFgvgUY4Tyl2UWyPoqsnI48IJa7ZrEiKVO6DhX3E8wex7CS3NmQjqDVrYwRkL7UP44BhzqKMlRNFUtHa6+dss449sqT7zXSdZXOFsLfmsixTt12TZ4CZnsdwUMKpTlBj/Qn5e+zTPwk/oR2awMYh5If8pBjnAV/5edw7BeeXR59CQIOxmL863F/tWrQQBuvWiBdeIkzE8wqSdsd6UzbXwqK9turq1+pHiuqKjDjG9ZlLlBcIIHOLxni9sk2C23IXNrXfucIQ8v2Tzprr66/l/GHGPVtjQ45KW09ahnWzefMmUI3K2E7VcgH/SyrDtRfN8Pi9sioecoFEgTxMizATrSOvs+Lea1gKm+2q9SN5sTVoJJfWhAShXt2frWjCxXHDMLzl5UQm/aDW+Ihf9EtJXAQLxfBAW+as8gkutVEujD/IGCStqiZ9s9B6QHPIlIEosVKsPcYkEucfGDBIsMZJMXACs+MtsTUG/2tjyvWtXruWWyh611WNiFUIZYLf82z4se1gROwMX7WPBL5U5XcLfpJyT8Oj7x9y2YGit9ZLM0ajirdsMdz6LcrcEchZ62x+pPu5ov+WSN+nXTHCsGHCVT1090Ba6t9MWKxOqR0sv59KY32tVHf9Nfff+0ILTDM/8raU0H76s3JND9mTcRfivyUFhxFOQRSi+hFufjY4r9enkO5JnUmtDGAj31EL0x1wla6CuAvWrPrioxG4IQDDdA7iEKjCtwUxFTj7D1YzLqa1Aeloz1EqpWptKeMh1zaaswcefNxdApdEuLAev/by7jhcuN6R/zP0u5Uex27MYBE+OxnsSBSRH1/0BDNsFEsF6oAIj8HzlAk1RCgczOAThbxBBNq7nmPhkto94CmGky+JvnCy30MPLR7TrWswum6spa0H/Vvjf1kDYv9VY+R6a3SDHQNVyepq7ts4THFS8/7HXoXoyzz2xovBmyW+8LvBkJjaSSXDXepurttmpCn0+wpxmm0hpFrlaV71bZvsXShHSQw36+ByFFG4a1FuPgWhzGGRSemWg777/m09olODQVlBwXHpPRuOYdcp4/DGSTFwArPjLbE1Bv9rY8r1rV67llsoetdVjYhVCGWC04h03bdgZdsf4Fw56drPk9+Lv5mSw1sJsUTkZZ7DT8KC6DMG0BkIM5AhrO+sDC/G4h4RCcdeodrGL5eANyK+zAGF5xShOWNZabN1llQKE+2tMu0DcRZ2zjuhQBg5yU14NbxcxCcu0UrIXvEBpDN0eeiGK0qyf6AuYKmxCVtM4IisHgPG0V4YbqRoo5fw9LV/j9ennoc3mW/fxEupfq1qqZcsgQwCFfD0EKeUYWXcahb5XAPMmKn3XsNRgldClczq+YRO0efvgZ74/cmrOWuIGChDa8l2NY1vWHHegT8A6Fgr/R1EXETB8byLbI67onZtUB1T7tmmNmXhcoyqLlmYJqTV44G1aV0JtVKx7fLgN6OfcRBbM0Evq9bGMqbgxhMitTqJXTm7k0fj3dWbSCjkUgs64l/GHQ8krqDN6wUI1lDuFKM4FFcT8cOIaZ3Kws+7wKCdpBOWGt37rXZo4thfEqYGeGK+yW+8mRMpiXeHbJMjXDjtEeDVA3+6TyXWntz5SCMqTXGFux7SnyDuXpK+6do8BjQhSWJFVFL+5Iauu2ckAAb1axCkfY/3EgWzSS20ot+AoL6Erx5QZr3RHTbHfhdIQR8WPo8EjSuMro5TW9ypdRZH+VimsiNefukliFNQgPwOGG1pYyr1t+2cJ3LZEjx0XPlD+BaMcTcZeeb7C0SbcLEskgXSRLsgkrvKL3PLq+r6vKKNMV41FvTeCKs9y3H5itq+VrfPGZtv6UnRJ4oIz/1uS3lY0f3idVc+w594Na1eu5ZbKHrXVY2IVQhlgtzhSbGECOaBqSGZ4EX0tGEK1UXapwoWRBsPahLP5jogLN3KYlsm8UhQv5o6BHN5cmpJQj2gWAFIfw2FObH4xNfgG2RqjDMZQ9sz9Ctk0nQvG0wDgAWjYntiSHuJyNInt8T0Fhaq90WisgbfHzgMrKatiFLuibE7syGoIVlnlop7gBqlUzYXWUhWJz6KjymmK5KnfIQs05+/3EFZtwHwNQp3XP2CY6cDcDWxgFMpX1S579ylhzouCg9TRdd6jjhzd3o68MJEcyalLFEUNnKjHOc1bCD+RM7lPKJhMaWUDcBzqow3vzJD6yNocphQj6FdT+MXWT4tEcSJorlsJCKO2RwnhAjbz2i9tCulsvv9J23GcROV0cjQvMOGlkMa484eDztAjuktCKaRqeyAEZPKeu8aILSMJ/FRiuEkiN/sYk3X9n96QL9WQqXfvQZZ0Vr+wsf97wgwdJYA3Mii6FV7dcpp4M6NeJJJ5OMtvV9JGZ+A94JprI07ulXljEqOgjTl1XntRqV2IJOTjfriNWs0iALw==\",\"hmac\":\"l7VL0n96wjNEMxunCaEG/gCLQ3o1UgWhe+VnYEZajWc=\"}";

                var request = (HttpWebRequest)WebRequest.Create("http://52.187.26.168/gsp/GSTR1Service.svc/SaveGSTR1");
                request.ContentType = "application/json; charset=utf-8"; //set the content type to JSON
                request.Accept = "application/json";
                request.Method = "POST"; //make an HTTP POST

                Request requestObj = new Request();
                requestObj.Clientid = "l7xxdf2b47b7d728426699a05c8d1ec33a60";
                requestObj.Username = "WeP";
                requestObj.ClientSecret = "30a28162eb024f6e859a12bbb9c31725";
                requestObj.Statecd = "11";
                requestObj.Txn = "returns";
                requestObj.AuthToken = "8a227e0ba56042a0acdf98b3477d2c03";
                requestObj.IpUsr = "l7xxdf12.8.9l.802b47b7d728426699a05c8d1ec33a60";
                requestObj.RequestToken = "";

                request.Headers["clientid"] = requestObj.Clientid;
                request.Headers["username"] = requestObj.Username;
                request.Headers["client-secret"] = requestObj.ClientSecret;
                request.Headers["state-cd"] = requestObj.Statecd;
                request.Headers["txn"] = requestObj.Txn;
                request.Headers["auth-token"] = requestObj.AuthToken;
                request.Headers["ip-usr"] = requestObj.AuthToken;

                //new WEP.GSP.Service.Blob.TableStorage(Constants.PartitionKey, Constants.RowKey
                //                                        , (int)WEP.GSP.Document.Stage.P1_Request_Sent_To_Service
                //                                        , requestObj.Clientid
                //                                        , requestObj.Username
                //                                        , requestObj.ClientSecret
                //                                        , requestObj.Statecd
                //                                        , requestObj.Txn
                //                                        , requestObj.AuthToken
                //                                        , requestObj.IpUsr
                //                                        , requestObj.RequestToken
                //                                        , Constants.GSTNStageTable
                //                                        , Constants.currentTime).
                //                                        InsertToTableStorage(string.Empty);

                byte[] byteArray = Encoding.UTF8.GetBytes(myEventHubMessage);
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);

                dataStream.Close();
                WebResponse response = request.GetResponse();
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                string responseFromServer = reader.ReadToEnd();
                Console.WriteLine(responseFromServer);
                reader.Close();
                dataStream.Close();
                response.Close();

                //new WEP.GSP.Service.Blob.TableStorage(Constants.PartitionKey, Constants.RowKey
                //                                        , (int)WEP.GSP.Document.Stage.P1_Message_sent_To_Master_Event_Hub
                //                                        , requestObj.Clientid
                //                                        , requestObj.Username
                //                                        , requestObj.ClientSecret
                //                                        , requestObj.Statecd
                //                                        , requestObj.Txn
                //                                        , requestObj.AuthToken
                //                                        , requestObj.IpUsr
                //                                        , requestObj.RequestToken
                //                                        , Constants.GSTNStageTable
                //                                        , Constants.currentTime).
                //                                        InsertToTableStorage(string.Empty);

            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
