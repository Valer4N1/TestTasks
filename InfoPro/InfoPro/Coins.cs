using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfoPro
{
    class Coins
    {
        public string country_label = "";
        public int amount_coins = 1;

        public Coins(string country_label, int amount_coins) { this.country_label = country_label; this.amount_coins = amount_coins; }
   }
}
