using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace InfoPro
{
    class Сountry
    {
		private static Random random = new Random((int)DateTime.Now.Ticks);

        private const int countries_x = 10;
        private const int countries_y = 10;
        private const int trade_limit_percent = 50;
        private const int types_currencies = 100;
        private const int neighboring_amount = 4;
        public List<int> sequence_neighbors = new List<int>();
        private List<Сountry> neighboring_countries = new List<Сountry>();
        private List<Coins> coffers = new List<Coins>();
		private int budget = 0;

        public string label = "";
        public int x = 0,
                   y = 0;

        public Сountry()	{}

        public Сountry(string label, int x, int y)
        {
            this.label = label;
            this.x = x;
            this.y = y;
        }

        public void Trade(int number_neighboring, ref int transaction_amount, ref Сountry seller, ref int currencies_buyer, ref int currencies_seller)
        {
			seller = neighboring_countries[sequence_neighbors[number_neighboring]];

			if(budget > 0)		transaction_amount = random.Next(0, budget);
			else				transaction_amount = 0;

			if(transaction_amount > 0)
			{
				int id_currency = 0;

				budget -= transaction_amount;

				for(int coins = 0; coins < transaction_amount; ++ coins)
				{
					id_currency = random.Next(0, coffers.Count() - 1);
					seller.Add_Coins(coffers[id_currency].country_label);
					if(-- coffers[id_currency].amount_coins <= 0)
						coffers.RemoveAt(id_currency);
				}

				currencies_buyer  = this  .coffers.Count();
				currencies_seller = seller.coffers.Count();
			}
        }

        public void Set_Budget()
		{
            int amount = coffers.Count();
            int coins = 0;
            for(int c = 0; c < amount; ++ c)    coins += coffers[c].amount_coins;
			budget = (int)((double)coins / 100.0 *  trade_limit_percent);
		}

		public void Reset_Coins(int coins)
		{
			coffers.Clear();
			Add_Coins(coins);
		}

		public void Add_Coins(int coins = 1)
		{
			coffers.Add(new Coins(label, coins));
		}

		private void Add_Coins(string country_label, int coins = 1)
		{
            int id = coffers.FindIndex(item => item.country_label == country_label);
            if(id != -1)        coffers[id].amount_coins += coins;
            else                coffers.Add(new Coins(country_label, coins));
		}

        public void Update_Sequence_Neighbors()
        {
			Get_Random_List_WO_Repeats(neighboring_amount, neighboring_amount, ref sequence_neighbors);
        }

        public void Search_Neighbors(List<Сountry> all_countries)
        {
			int x1 = 0, y1 = 0,
				x2 = 0, y2 = 0;
			
			Search_Neighbors_Сoordinates(x, ref x1, ref x2, countries_x);
			Search_Neighbors_Сoordinates(y, ref y1, ref y2, countries_y);

			neighboring_countries.Add(all_countries[this.y  * 10 +      x1]);
			neighboring_countries.Add(all_countries[this.y  * 10 +      x2]);
			neighboring_countries.Add(all_countries[     y1 * 10 + this.x ]);
			neighboring_countries.Add(all_countries[     y2 * 10 + this.x ]);
        }

// =======================================================================================================================================================================

		private void Search_Neighbors_Сoordinates(int xy, ref int xy1, ref int xy2, int countries_xy)
		{
			switch(xy)
			{
				case 0:
				{
					xy1 = xy + countries_xy - 1;
					xy2 = xy + 1;
					break;
				}
				case 9:
				{
					xy1 = xy - 1;
					xy2 = xy - countries_xy + 1;
					break;
				}
				default:
				{
					xy1 = xy - 1;
					xy2 = xy + 1;
					break;
				}
			}
		}

        private void Get_Random_List_WO_Repeats(int need, int max, ref List<int> result)
        {
			List<int> temp = new List<int>();

	        need = (need < max) ? need : max;

			for(int i = 0             ; i < need; ++ i)		temp  .Add(0);
			for(int i = result.Count(); i < need; ++ i)		result.Add(0);

	        int last_id = max - 1;
	        for(int i = 0; i < need; ++ i)
		        result[i] = temp[i] = random.Next(0, last_id - i);

	        for(int i = 1; i < need; ++ i)
		        for(int pre = i - 1; pre >= 0 ; -- pre)
			        if(result[i] >= temp[pre])
				        result[i] ++;
        }
    }
}
