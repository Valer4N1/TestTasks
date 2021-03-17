using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace InfoPro
{
    public partial class Form1 : Form
    {
		private static Random random = new Random((int)DateTime.Now.Ticks);

        private const int countries_x = 10;
        private const int countries_y = 10;
        private const int total_countries = countries_x * countries_y; 
        private const int amount_deals = total_countries * 4;
        private List<Сountry> countries = new List<Сountry>();
		private List<int> sequence_countries = new List<int>();
        private int country_line_number = 0;
		private bool running   = false;
        private bool completed = true ;
        private int deal  = 0;
        private int month = 0;
        private int year  = 0;
        private int max_currencies = 0;

        private const float country_width  = 80.0F;
        private const float country_height = 80.0F;
		private const float width_pen = 2.0F;
		private const int font_size = 12;
        private Graphics g;
        private Bitmap bmp;

		private int default_timer_interval = 10;
		
		Сountry pre_seller = new Сountry();
		Сountry pre_buyer  = new Сountry();

		private string[] months = new string[] {"Январь",
                                                "Февраль",
                                                "Март",
                                                "Апрель",
                                                "Май",
                                                "Июнь",
                                                "Июль",
                                                "Август",
                                                "Сентябрь",
                                                "Октябрь",
                                                "Ноябрь",
                                                "Декабрь"};
		
        public Form1()
        {
            InitializeComponent();
			
			System.Diagnostics.Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;
			this.DoubleBuffered = true;

			timer1.Interval = default_timer_interval;

            List<int> country_labels = new List<int>();
            Get_Random_List_WO_Repeats(total_countries, total_countries, ref country_labels    );
            Get_Random_List_WO_Repeats(total_countries, total_countries, ref sequence_countries);
            for(int y = 0; y < countries_y; ++ y)
                for(int x = 0; x < countries_x; ++ x)
                    countries.Add(new Сountry(country_labels[y * countries_x + x].ToString(), x, y));

            bmp = new Bitmap(countries_x * (int)country_width ,
							 countries_y * (int)country_height);
            g = Graphics.FromImage(bmp);
            for(int с = 0; с < total_countries; ++ с)
			{
				Repaint_Country(countries[с], Brushes.White);
				countries[с].Search_Neighbors(countries);
			}
            pictureBox1.Image = bmp;
        }

        private void button_start_Click(object sender, EventArgs e)
        {
			if(!running)	{ running = true ; button_start.Text = "Остановить"; Check_Reset()							 ; }
			else			{ running = false; button_start.Text = "Запустить" ; timer1.Interval = default_timer_interval; }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
			if( running
			&& !completed)
			{
				timer1.Interval = (int)numericUpDown1.Value;

				Сountry buyer  = countries[sequence_countries[country_line_number ++]];
				Сountry seller = new Сountry();

				int currencies_buyer  = 0;
				int currencies_seller = 0;

				int transaction_amount  = 0;
				buyer.Trade(deal / total_countries, ref transaction_amount, ref seller, ref currencies_buyer, ref currencies_seller);

				if(pre_seller != null)
				{
					Repaint_Country(pre_seller, Brushes.White);
					Repaint_Country(pre_buyer , Brushes.White);
				}
                pre_seller = seller;
                pre_buyer  = buyer ;

                if(transaction_amount > 0)
                {
					Repaint_Country(seller, Brushes.Green);
					Repaint_Country(buyer , Brushes.Red  );
				    Draw_String("+" + transaction_amount.ToString(),
								    seller.x * country_width  + country_width  * 0.25F - (float)font_size,
								    seller.y * country_height + country_height * 0.25F - (float)font_size,
									    font_size);

				    Draw_String("-" + transaction_amount.ToString(),
								    buyer .x * country_width  + country_width  * 0.75F - (float)font_size,
								    buyer .y * country_height + country_height * 0.75F - (float)font_size,
									    font_size);

                    pictureBox1.Image = bmp;

                }

                if(max_currencies < currencies_buyer)       { max_currencies = currencies_buyer ;		label3.Text = buyer .label; }
                if(max_currencies < currencies_seller)      { max_currencies = currencies_seller;		label3.Text = seller.label; }

				label4.Text = max_currencies.ToString();

                if(max_currencies == total_countries)
				{
					button_start.Text = "Запустить" ;
                    label2      .Text = "Победитель";
					completed = true;
                    running   = false;
				}
				else
					if(++ deal % total_countries == 0)
					{
						country_line_number = 0;

						Get_Random_List_WO_Repeats(total_countries, total_countries, ref sequence_countries);

						if(deal >= amount_deals)
						{
							deal = 0;

							if(++ month >= 12)
							{
								month = 0;
								year ++;
                                label8.Text = "Год: " + year.ToString();
								int amount_annual_replenishment = (int)numericUpDown2.Value;
								for(int с = 0; с < total_countries; ++ с)   countries[с].Add_Coins(amount_annual_replenishment);
							}
							New_Month(month);
						}
					}
			}
        }

        private void Check_Reset()
        {
            if(completed)
            {
				completed = false;
				label2.Text = "Лидер";
				label8.Text = "Год: 0";
				country_line_number = 0;
                deal  = 0;
                month = 0;
                year  = 0;
				max_currencies = 0;
				int amount_annual_replenishment = (int)numericUpDown2.Value;
                for(int с = 0; с < total_countries; ++ с)       countries[с].Reset_Coins(amount_annual_replenishment);
				New_Month(month);
            }
        }

		private void New_Month(int month)
		{
            label6.Text = months[month];
			for(int с = 0; с < total_countries; ++ с)
			{
				countries[с].Set_Budget();
				countries[с].Update_Sequence_Neighbors();
			}
		}

		private void Repaint_Country(Сountry country, Brush color)
		{
					g.FillRectangle(color,
										country.x * country_width ,
										country.y * country_height,
												    country_width ,
												    country_height);
					g.DrawRectangle(new Pen(Color.Black, width_pen),
										country.x * country_width ,
										country.y * country_height,
												    country_width ,
												    country_height);
					Draw_String(country.label,
										country.x * country_width  + country_width  / 2.0F - (float)font_size,
										country.y * country_height + country_height / 2.0F - (float)font_size,
											font_size);
		}

		public void Draw_String(string draw_string, float x, float y, int size)
		{
			System.Drawing.Font draw_font           = new System.Drawing.Font        ("Arial", size);
			System.Drawing.SolidBrush draw_brush    = new System.Drawing.SolidBrush  (System.Drawing.Color.Black);
			System.Drawing.StringFormat draw_format = new System.Drawing.StringFormat();
			g.DrawString(draw_string, draw_font, draw_brush, x, y, draw_format);
			draw_font .Dispose();
			draw_brush.Dispose();
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
