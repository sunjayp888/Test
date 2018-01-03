using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.qrcode;
using UpStoxWindows.Helper;
using UpstoxNet;

namespace UpStoxWindows
{
    public partial class Form1 : Form
    {
        Upstox upstox = new Upstox();

        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click_1(object sender, EventArgs e)
        {
            upstox.Api_Key = "hKiZa1pkqf8012fndXZjE4RsT53xCnalaZWHPRKu";
            upstox.Api_Secret = "ttfr9t59rg";
            upstox.Redirect_Url = "http://upstox.com";
            upstox.Stream_Mode = UpstoxNet.Mode.Full;
            try
            {
                upstox.Login();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                throw;
            }
        }

        private void Button2_Click_1(object sender, EventArgs e)
        {

        }

        private void Button17_Click(object sender, EventArgs e)
        {
            try
            {
                upstox.GetAccessToken();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void Button18_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(upstox.Authorization_Status ? "Access Token Received" : "Access Token not Received");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void Button19_Click(object sender, EventArgs e)
        {
            try
            {
                upstox.GetMasterContract();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show(upstox.Symbol_Download_Status ? "Symbols Downloaded" : "Symbols not Downloaded or in Progress");
        }

        private void button24_Click(object sender, EventArgs e)
        {
            const int differenceInTime = 2;
            const double profit = 1;
            try
            {
                var symbolsNse = upstox.GetSymbols("NSE_EQ");
                var symbols = MISTrade().ToList();
                var stockListResultData = new List<Stock>();
                const string stocks = "";
                var positions = upstox.GetPositions().Split('\n').Skip(1).ToList();

                var result = PlaceOrder(symbols, symbolsNse, differenceInTime, stocks, positions, profit, stockListResultData);

                var list = new BindingList<Stock>(result.ToList());
                var source = new BindingSource(list, null);
                dataGridViewStock.DataSource = source;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private List<Stock> PlaceOrder(List<string> symbols, string symbolsNse, int differenceInTime, string stocks, List<string> positions, double profit, List<Stock> stockListResultData)
        {
            var actualResultTestList = new List<Stock>();
           
            var marginProfit = 2;
            var marginStopLoss = 5;
            foreach (var symbol in symbols)
            {
                if (symbolsNse.Contains(symbol))
                {
                    //var fromDateTime = new DateTime(2018,01,01,9,16,0);
                    //var toDateTime = fromDateTime.AddMinutes(1);
                    var fromDateTime = DateTime.Now.AddMinutes(-differenceInTime).AddSeconds(-DateTime.Now.Second);
                    var toDateTime = fromDateTime.AddMinutes(1);

                    //   new StartDateTime(StartDateTime.Now.Year, StartDateTime.Now.Month, StartDateTime.Now.Day, StartDateTime.Now.Hour, StartDateTime.Now.Minute + differenceInTime, 0);
                    var data = upstox.GetHistData("NSE_EQ", symbol, "1MINUTE", fromDateTime, toDateTime, false).ToList();
                    var stockListData = new List<Stock>();

                    foreach (var stock in data.Skip(1))
                    {
                        var stocksplitData = stock.Split(',');
                        stockListData.Add(new Stock()
                        {
                            Exchange = "NSE_EQ",
                            Symbol = symbol,
                            StartDateTime = DateTime.Parse(stocksplitData[0]),
                            EndDateTime = toDateTime,
                            Open = stocksplitData[1],
                            High = stocksplitData[2],
                            Low = stocksplitData[3],
                            Close = stocksplitData[4],
                            Volume = stocksplitData[5],
                            CP = stocksplitData[6],
                            StockString = stocks
                        });
                    }
                    var timeStartData = stockListData.FirstOrDefault(c => c.StartDateTime.Hour == fromDateTime.Hour && c.StartDateTime.Minute == fromDateTime.Minute);
                    var timeEndData = stockListData.FirstOrDefault(c => c.StartDateTime.Hour == toDateTime.Hour && c.StartDateTime.Minute == toDateTime.Minute);

                    //var timeofPlacingOrder = stockListData.FirstOrDefault(c => c.StartDateTime.Hour == toDateTime.Hour && c.StartDateTime.Minute == toDateTime.Minute + 1);
                    var positionList = (from item in positions
                                        let positionData = item.Split(',')
                                        where item != string.Empty
                                        select new Stock()
                                        {
                                            Exchange = positionData[0],
                                            Symbol = positionData[2],
                                            NetQuantity = Convert.ToDouble(positionData[14])
                                        }).ToList();

                    if (timeStartData != null && timeEndData != null)
                    {
                        var profitHighMargin = Convert.ToDouble(timeEndData.High) - Convert.ToDouble(timeStartData.High);
                        var profitLowMargin = Convert.ToDouble(timeEndData.Low) - Convert.ToDouble(timeStartData.Low) > 0;
                        var exactTimeData = upstox.GetHistData("NSE_EQ", symbol, "1MINUTE", toDateTime.AddMinutes(1), toDateTime.AddMinutes(1), false).ToList();
                        var ltp = upstox.GetSnapLtp("NSE_EQ", timeStartData.Symbol);
                        var isLtpUp = ltp > Convert.ToDouble(timeEndData.High);
                        var isLtpDown = ltp > Convert.ToDouble(timeEndData.Low);
                        if (profitHighMargin >= profit)
                        {

                            stockListResultData.Add(new Stock()
                            {
                                Exchange = "NSE_EQ",
                                Symbol = symbol,
                                StartDateTime = timeStartData.StartDateTime,
                                EndDateTime = toDateTime,
                                Open = timeStartData.Open,
                                High = timeStartData.High,
                                Low = timeStartData.Low,
                                Close = timeStartData.Close,
                                Call = "BUY",
                                Volume = timeStartData.Volume,
                                CP = timeStartData.CP,
                                StockString = stocks,
                                BuyPrice = Convert.ToDouble(timeEndData.Close)
                            });
                            var buyPrice = Convert.ToDouble(timeEndData.Close);
                            var currentBuyLtp = upstox.GetSnapLtp("NSE_EQ", timeStartData.Symbol);
                            //Place order if not in position


                            if ((positionList.Any(dd => dd.Symbol == symbol && dd.NetQuantity == 0)) || (positionList.All(dd => dd.Symbol != symbol)) && currentBuyLtp > Convert.ToDouble(timeEndData.High))
                                upstox.PlaceOCO("NSE_EQ", symbol, "B", 1, currentBuyLtp, marginProfit, marginStopLoss, 1);
                        }
                        if (profitHighMargin <= -profit )
                        {
                            stockListResultData.Add(new Stock()
                            {
                                Exchange = "NSE_EQ",
                                Symbol = symbol,
                                StartDateTime = timeStartData.StartDateTime,
                                EndDateTime = toDateTime,
                                Open = timeStartData.Open,
                                High = timeStartData.High,
                                Low = timeStartData.Low,
                                Close = timeStartData.Close,
                                Call = "SELL",
                                Volume = timeStartData.Volume,
                                CP = timeStartData.CP,
                                StockString = stocks,
                                BuyPrice = Convert.ToDouble(timeEndData.Close)
                            });
                            var currentltp = upstox.GetSnapLtp("NSE_EQ", timeStartData.Symbol);
                            var buyPrice = Convert.ToDouble(timeEndData.Close);
                            //Place order if not in position
                            if ((positionList.Any(dd => dd.Symbol == symbol && dd.NetQuantity == 0)) || (positionList.All(dd => dd.Symbol != symbol)) && Convert.ToDouble(timeEndData.Low) < currentltp)
                                upstox.PlaceOCO("NSE_EQ", symbol, "S", 1, buyPrice, marginProfit, marginStopLoss, 1);
                        }
                    }


                }
            }
             return stockListResultData;
            //foreach (var buyStock in stockListResultData)
            //{
            //    var buyFlag = true;
            //    var sellFlag = true;
            //    var stockListData2 = new List<Stock>();
            //    var symboldata = upstox.GetHistData("NSE_EQ", buyStock.Symbol, "1MINUTE", fromDateTime, toDateTime, false).ToList();
            //    foreach (var stock in symboldata.Skip(1))
            //    {
            //        var stocksplitData = stock.Split(',');
            //        stockListData2.Add(new Stock()
            //        {
            //            Exchange = "NSE_EQ",
            //            Symbol = buyStock.Symbol,
            //            StartDateTime = DateTime.Parse(stocksplitData[0]),
            //            EndDateTime = toDateTime,
            //            Open = stocksplitData[1],
            //            High = stocksplitData[2],
            //            Low = stocksplitData[3],
            //            Close = stocksplitData[4],
            //            Volume = stocksplitData[5],
            //            CP = stocksplitData[6],
            //            StockString = stocks
            //        });
            //    }
            //    var dataToTest = stockListData2.Where(e => e.StartDateTime > toDateTime);
            //    foreach (var stock in dataToTest)
            //    {
            //        if (buyStock.Call.ToLower() == "buy")
            //        {
            //            if (Convert.ToDouble(stock.High) >= buyStock.BuyPrice + marginProfit && buyFlag)
            //            {
            //                actualResultTestList.Add(new Stock()
            //                {
            //                    BuyPrice = buyStock.BuyPrice,
            //                    Low = buyStock.Low,
            //                    Symbol = buyStock.Symbol,
            //                    Profit = marginProfit
            //                });
            //                buyFlag = false;
            //            }
            //            if (Convert.ToDouble(stock.Low) <= buyStock.BuyPrice - marginStopLoss && buyFlag)
            //            {
            //                actualResultTestList.Add(new Stock()
            //                {
            //                    BuyPrice = buyStock.BuyPrice,
            //                    Low = buyStock.Low,
            //                    Symbol = buyStock.Symbol,
            //                    Profit = -marginStopLoss
            //                });
            //                buyFlag = false;
            //            }

            //        }
            //        if (buyStock.Call.ToLower() == "sell")
            //        {
            //            if (Convert.ToDouble(stock.Low) <= buyStock.BuyPrice - marginProfit && sellFlag)
            //            {
            //                actualResultTestList.Add(new Stock()
            //                {
            //                    BuyPrice = buyStock.BuyPrice,
            //                    Low = buyStock.Low,
            //                    Symbol = buyStock.Symbol,
            //                    Profit = marginProfit
            //                });
            //                sellFlag = false;
            //            }
            //            if (Convert.ToDouble(stock.Low) >= buyStock.BuyPrice + marginStopLoss && sellFlag)
            //            {
            //                actualResultTestList.Add(new Stock()
            //                {
            //                    BuyPrice = buyStock.BuyPrice,
            //                    Low = buyStock.Low,
            //                    Symbol = buyStock.Symbol,
            //                    Profit = -marginStopLoss
            //                });
            //                sellFlag = false;
            //            }
            //        }
            //    }
            //}
            return actualResultTestList;
        }


        public string[] MISTrade()
        {
            var allIntradayData = "3MINDIA,AARTIIND,ABAN,ABB,ABFRL,ACC,ADANIENT,ADANIPORTS,ADANIPOWER,AJANTPHARM,AKZOINDIA,ALBK,ALKEM,ALLCARGO,AMARAJABAT,AMBUJACEM,ANDHRABANK,APLLTD,APOLLOHOSP,APOLLOTYRE,ARVIND,ASAHIINDIA,ASHOKLEY,ASIANPAINT,ASTRAZEN,ATFL,ATUL,AUBANK,AUROPHARMA,AUTOAXLES,AXISBANK,BAJAJ-AUTO,BAJAJCORP,BAJAJELEC,BAJAJFINSV,BAJAJHIND,BAJAJHLDNG,BAJFINANCE,BALKRISIND,BALRAMCHIN,BANCOINDIA,BANKBARODA,BANKBEES,BANKINDIA,BATAINDIA,BEL,BEML,BERGEPAINT,BGRENERGY,BHARATFIN,BHARATFORG,BHARTIARTL,BHEL,BIOCON,BLUEDART,BOSCHLTD,BPCL,BRITANNIA,BRNL,BSE,CADILAHC,CANBK,CANFINHOME,CAPACITE,CAPF,CASTROLIND,CDSL,CEATLTD,CENTRALBK,CENTURYPLY,CENTURYTEX,CESC,CGPOWER,CHENNPETRO,CHOLAFIN,CIPLA,COALINDIA,COCHINSHIP,COFFEEDAY,COLPAL,CONCOR,COROMANDEL,COX&KINGS,CRISIL,CROMPTON,CUB,CUMMINSIND,CYIENT,DABUR,DALMIABHA,DBCORP,DBREALTY,DCBBANK,DCMSHRIRAM,DHFL,DIAMONDYD,DISHTV,DIVISLAB,DIXON,DLF,DMART,DRREDDY,ECLERX,EDELWEISS,EICHERMOT,EIDPARRY,EIHOTEL,EMAMILTD,ENDURANCE,ENGINERSIN,EQUITAS,ERIS,ESCORTS,EVEREADY,EXIDEIND,FEDERALBNK,FEL,FINCABLES,FORTIS,FRETAIL,GAIL,GANECOS,GATI,GDL,GEPIL,GESHIP,GICHSGFIN,GICRE,GILLETTE,GLAXO,GLENMARK,GMRINFRA,GNA,GNFC,GODREJAGRO,GODREJCP,GODREJIND,GODREJPROP,GOLDBEES,GPPL,GRANULES,GRASIM,GREAVESCOT,GRUH,GSFC,GSKCONS,GSPL,GTPL,GUJALKALI,GUJFLUORO,GUJGASLTD,HAVELLS,HCC,HCLTECH,HDFC,HDFCBANK,HDIL,HEIDELBERG,HEROMOTOCO,HEXAWARE,HGS,HIKAL,HINDALCO,HINDPETRO,HINDUNILVR,HINDZINC,HONAUT,HOTELEELA,HSCL,HSIL,HUDCO,IBREALEST,IBULHSGFIN,ICICIBANK,ICICIGI,ICICIPRULI,ICIL,IDBI,IDEA,IDFC,IDFCBANK,IEX,IFCI,IGL,IGPL,IIFL,IL&FSTRANS,INDHOTEL,INDIACEM,INDIANB,INDIGO,INDUSINDBK,INFIBEAM,INFRATEL,INFY,INOXLEISUR,INOXWIND,INTELLECT,IOB,IOC,IPCALAB,IRB,ITC,JAGRAN,JAMNAAUTO,JETAIRWAYS,JINDALPOLY,JINDALSTEL,JISLJALEQS,JKCEMENT,JKIL,JKPAPER,JKTYRE,JPASSOCIAT,JSL,JSLHISAR,JSWENERGY,JSWSTEEL,JUBILANT,JUBLFOOD,JUSTDIAL,JYOTHYLAB,KAJARIACER,KALPATPOWR,KANSAINER,KARURVYSYA,KEC,KEI,KESORAMIND,KHADIM,KIRLOSENG,KOKUYOCMLN,KOTAKBANK,KOTAKNIFTY,KPIT,KRBL,KSCL,KTKBANK,KWALITY,L&TFH,LALPATHLAB,LEEL,LGBBROSLTD,LICHSGFIN,LINDEINDIA,LIQUIDBEES,LOVABLE,LT,LUPIN,M&M,M&MFIN,M100,M50,MAGMA,MAHINDCIE,MAHLOG,MAJESCO,MANALIPETC,MANAPPURAM,MANGALAM,MANINFRA,MANPASAND,MARICO,MARKSANS,MARUTI,MASFIN,MATRIMONY,MCDOWELL-N,MCLEODRUSS,MCX,MEGH,MERCATOR,MFSL,MGL,MINDAIND,MINDTREE,MOIL,MOLDTKPAC,MOTHERSUMI,MPHASIS,MRF,MRPL,MUKANDLTD,MUTHOOTFIN,NATIONALUM,NAUKRI,NBCC,NCC,NESTLEIND,NETWORK18,NFL,NH,NHPC,NIACL,NIF100IWIN,NIFTYBEES,NIFTYIWIN,NIITLTD,NIITTECH,NLCINDIA,NMDC,NOCIL,NRBBEARING,NTPC,OBEROIRLTY,OFSS,OIL,OMAXE,ONGC,ORIENTBANK,ORIENTCEM,PAGEIND,PARAGMILK,PCJEWELLER,PEL,PERSISTENT,PETRONET,PFC,PFIZER,PGHH,PHOENIXLTD,PIDILITIND,PIIND,PNB,PNBHOUSING,POLYPLEX,POWERGRID,PRAKASH,PRESTIGE,PTC,PVR,QUICKHEAL,RADICO,RADIOCITY,RAJESHEXPO,RALLIS,RAMCOCEM,RAMCOIND,RAYMOND,RBLBANK,RCF,RCOM,RECLTD,RELAXO,RELCAPITAL,RELIANCE,RELINFRA,REPCOHOME,RICOAUTO,RKFORGE,RNAM,RNAVAL,ROLTA,RPOWER,SADBHAV,SAIL,SALASAR,SALZERELEC,SANGHIIND,SANOFI,SAREGAMA,SBILIFE,SBIN,SCHAND,SCI,SHANKARA,SHARDAMOTR,SHRIRAMCIT,SICAL,SIEMENS,SINTEX,SIS,SJVN,SKFINDIA,SNOWMAN,SOBHA,SOLARINDS,SOUTHBANK,SPARC,SPTL,SREINFRA,SRF,SRTRANSFIN,STAR,SUNDARMFIN,SUNDRMFAST,SUNPHARMA,SUNTECK,SUNTV,SUPREMEIND,SUZLON,SYMPHONY,SYNDIBANK,SYNGENE,TATACHEM,TATACOFFEE,TATACOMM,TATAELXSI,TATAGLOBAL,TATAINVEST,TATAMOTORS,TATAMTRDVR,TATAPOWER,TATASPONGE,TATASTEEL,TBZ,TCI,TCS,TECHM,TEJASNET,THERMAX,THOMASCOOK,THYROCARE,TIFIN,TIRUMALCHM,TITAN,TNPETRO,TNPL,TORNTPHARM,TORNTPOWER,TRENT,TRIDENT,TTKPRESTIG,TV18BRDCST,TVSMOTOR,TWL,UBL,UCOBANK,UJJIVAN,ULTRACEMCO,UNIONBANK,UPL,VAKRANGEE,VEDL,VENKEYS,VGUARD,VIJAYABANK,VISHNU,VOLTAS,VTL,WABCOINDIA,WELENT,WHIRLPOOL,WIPRO,WOCKPHARMA,WONDERLA,YESBANK,ZEEL";
            //share below 700
            var shareBelow700 = "ABAN,ABFRL,ADANIENT,ADANIPORTS,ADANIPOWER,ALBK,ALLCARGO,AMBUJACEM,ANDHRABANK,APLLTD,APOLLOTYRE,ARVIND,ASAHIINDIA,ASHOKLEY,ATFL,AUBANK,AUROPHARMA,AXISBANK,BAJAJCORP,BAJAJELEC,BAJAJHIND,BALRAMCHIN,BANCOINDIA,BANKBARODA,BANKINDIA,BEL,BERGEPAINT,BGRENERGY,BHARATFORG,BHARTIARTL,BHEL,BIOCON,BPCL,BRNL,CADILAHC,CANBK,CANFINHOME,CAPACITE,CAPF,CASTROLIND,CDSL,CENTRALBK,CENTURYPLY,CGPOWER,CHENNPETRO,CIPLA,COALINDIA,COCHINSHIP,COFFEEDAY,COROMANDEL,COX&KINGS,CROMPTON,CUB,CYIENT,DABUR,DBCORP,DBREALTY,DCBBANK,DCMSHRIRAM,DHFL,DISHTV,DLF,EDELWEISS,EIDPARRY,EIHOTEL,ENGINERSIN,EQUITAS,ESCORTS,EVEREADY,EXIDEIND,FEDERALBNK,FEL,FINCABLES";
            //Share below 1000
            var shareBelow1000 =
                "ABAN,ABFRL,ADANIENT,ADANIPORTS,ADANIPOWER,ALBK,ALLCARGO,AMARAJABAT,AMBUJACEM,ANDHRABANK,APLLTD,APOLLOTYRE,ARVIND,ASAHIINDIA,ASHOKLEY,ATFL,AUBANK,AUROPHARMA,AXISBANK,BAJAJCORP,BAJAJELEC,BAJAJHIND,BALRAMCHIN,BANCOINDIA,BANKBARODA,BANKINDIA,BATAINDIA,BEL,BERGEPAINT,BGRENERGY,BHARATFORG,BHARTIARTL,BHEL,BIOCON,BPCL,BRNL,BSE,CADILAHC,CANBK,CANFINHOME,CAPACITE,CAPF,CASTROLIND,CDSL,CENTRALBK,CENTURYPLY,CGPOWER,CHENNPETRO,CIPLA,COALINDIA,COCHINSHIP,COFFEEDAY,COROMANDEL,COX&KINGS,CROMPTON,CUB,CUMMINSIND,CYIENT,DABUR,DBCORP,DBREALTY,DCBBANK,DCMSHRIRAM,DHFL,DISHTV,DLF,EDELWEISS,EIDPARRY,EIHOTEL,ENGINERSIN,EQUITAS,ERIS,ESCORTS,EVEREADY,EXIDEIND,FEDERALBNK,FEL,FINCABLES,FORTIS,FRETAIL,GAIL,GANECOS,GATI,GDL,GEPIL,GESHIP,GICHSGFIN,GICRE,GLENMARK,GMRINFRA,GNA,GNFC,GODREJAGRO,GODREJCP,GODREJIND,GODREJPROP,GPPL,GRANULES,GREAVESCOT,GRUH,GSFC,GSPL,GTPL,GUJALKALI,GUJFLUORO,GUJGASLTD,HAVELLS,HCC,HCLTECH,HDIL,HEIDELBERG,HEXAWARE,HGS,HIKAL,HINDALCO,HINDPETRO,HINDZINC,HOTELEELA,HSCL,HSIL,HUDCO,IBREALEST,ICICIBANK,ICICIGI,ICICIPRULI,ICIL,IDBI,IDEA,IDFC,IDFCBANK,IFCI,IGL,IGPL,IIFL,IL&FSTRANS,INDHOTEL,INDIACEM,INDIANB,INFIBEAM,INFRATEL,INOXLEISUR,INOXWIND,INTELLECT,IOB,IOC,IPCALAB,IRB,ITC,JAGRAN,JAMNAAUTO,JETAIRWAYS,JINDALPOLY,JINDALSTEL,JISLJALEQS,JKIL,JKPAPER,JKTYRE,JPASSOCIAT,JSL,JSLHISAR,JSWENERGY,JSWSTEEL,JUBILANT,JUSTDIAL,JYOTHYLAB,KAJARIACER,KALPATPOWR,KANSAINER,KARURVYSYA,KEC,KEI,KESORAMIND,KHADIM,KIRLOSENG,KOKUYOCMLN,KOTAKNIFTY,KPIT,KRBL,KSCL,KTKBANK,KWALITY,L&TFH,LALPATHLAB,LEEL,LICHSGFIN,LINDEINDIA,LOVABLE,LUPIN,M&M,M&MFIN,M100,M50,MAGMA,MAHINDCIE,MAHLOG,MAJESCO,MANALIPETC,MANAPPURAM,MANGALAM,MANINFRA,MANPASAND,MARICO,MARKSANS,MASFIN,MATRIMONY,MCLEODRUSS,MCX,MEGH,MERCATOR,MFSL,MINDTREE,MOIL,MOLDTKPAC,MOTHERSUMI,MPHASIS,MRPL,MUKANDLTD,MUTHOOTFIN,NATIONALUM,NBCC,NCC,NETWORK18,NFL,NH,NHPC,NIACL,NIF100IWIN,NIFTYIWIN,NIITLTD,NIITTECH,NLCINDIA,NMDC,NOCIL,NRBBEARING,NTPC,OBEROIRLTY,OIL,OMAXE,ONGC,ORIENTBANK,ORIENTCEM,PARAGMILK,PCJEWELLER,PERSISTENT,PETRONET,PFC,PHOENIXLTD,PIDILITIND,PIIND,PNB,POLYPLEX,POWERGRID,PRAKASH,PRESTIGE,PTC,QUICKHEAL,RADICO,RADIOCITY,RAJESHEXPO,RALLIS,RAMCOCEM,RAMCOIND,RBLBANK,RCF,RCOM,RECLTD,RELAXO,RELCAPITAL,RELIANCE,RELINFRA,REPCOHOME,RICOAUTO,RKFORGE,RNAM,RNAVAL,ROLTA,RPOWER,SADBHAV,SAIL,SALASAR,SALZERELEC,SANGHIIND,SAREGAMA,SBILIFE,SBIN,SCHAND,SCI,SICAL,SINTEX,SJVN,SNOWMAN,SOBHA,SOUTHBANK,SPARC,SPTL,SREINFRA,STAR,SUNDRMFAST,SUNPHARMA,SUNTECK,SUZLON,SYNDIBANK,SYNGENE,TATACHEM,TATACOFFEE,TATACOMM,TATAELXSI,TATAGLOBAL,TATAINVEST,TATAMOTORS,TATAMTRDVR,TATAPOWER,TATASPONGE,TATASTEEL,TBZ,TCI,TECHM,TEJASNET,THOMASCOOK,THYROCARE,TIFIN,TITAN,TNPETRO,TNPL,TORNTPOWER,TRENT,TRIDENT,TV18BRDCST,TVSMOTOR,TWL,UCOBANK,UJJIVAN,UNIONBANK,UPL,VAKRANGEE,VEDL,VGUARD,VIJAYABANK,VOLTAS,WELENT,WIPRO,WOCKPHARMA,WONDERLA,YESBANK,ZEEL";
            var shareBelow500 = "ABAN,ABFRL,ADANIENT,ADANIPORTS,ADANIPOWER,ALBK,ALLCARGO,AMBUJACEM,ANDHRABANK,APOLLOTYRE,ARVIND,ASAHIINDIA,ASHOKLEY,BAJAJCORP,BAJAJELEC,BAJAJHIND,BALRAMCHIN,BANCOINDIA,BANKBARODA,BANKINDIA,BEL,BERGEPAINT,BGRENERGY,BHEL,BRNL,CADILAHC,CANBK,CANFINHOME,CAPACITE,CASTROLIND,CDSL,CENTRALBK,CENTURYPLY,CGPOWER,CHENNPETRO,COALINDIA,COFFEEDAY,COX&KINGS,CROMPTON,CUB,DABUR,DBCORP,DBREALTY,DCBBANK,DISHTV,DLF,EDELWEISS,EIDPARRY,EIHOTEL,ENGINERSIN,EQUITAS,EVEREADY,EXIDEIND,FEDERALBNK,FEL,FORTIS,GANECOS,GATI,GDL,GESHIP,GICHSGFIN,GMRINFRA,GNA,GNFC,GPPL,GRANULES,GREAVESCOT,GRUH,GSFC,GSPL,GTPL,HCC,HDIL,HEIDELBERG,HEXAWARE,HIKAL,HINDALCO,HINDPETRO,HINDZINC,HOTELEELA,HSCL,HUDCO,IBREALEST,ICICIBANK,ICICIPRULI,ICIL,IDBI,IDEA,IDFC,IDFCBANK,IFCI,IGL,IL&FSTRANS,INDHOTEL,INDIACEM,INDIANB,INFIBEAM,INFRATEL,INOXLEISUR,INOXWIND,INTELLECT,IOB,IOC,IRB,ITC,JAGRAN,JAMNAAUTO,JINDALPOLY,JINDALSTEL,JISLJALEQS,JKIL,JKPAPER,JKTYRE,JPASSOCIAT,JSL,JSLHISAR,JSWENERGY,JSWSTEEL,JYOTHYLAB,KALPATPOWR,KARURVYSYA,KEC,KEI,KESORAMIND,KIRLOSENG,KOKUYOCMLN,KOTAKNIFTY,KPIT,KTKBANK,KWALITY,L&TFH,LEEL,LOVABLE,M&MFIN,M100,M50,MAGMA,MAHINDCIE,MAHLOG,MANALIPETC,MANAPPURAM,MANGALAM,MANINFRA,MANPASAND,MARICO,MARKSANS,MCLEODRUSS,MEGH,MERCATOR,MOIL,MOLDTKPAC,MOTHERSUMI,MRPL,MUKANDLTD,MUTHOOTFIN,NATIONALUM,NBCC,NCC,NETWORK18,NFL,NH,NHPC,NIF100IWIN,NIFTYIWIN,NIITLTD,NLCINDIA,NMDC,NOCIL,NRBBEARING,NTPC,OBEROIRLTY,OIL,OMAXE,ONGC,ORIENTBANK,ORIENTCEM,PARAGMILK,PCJEWELLER,PETRONET,PFC,PNB,POWERGRID,PRAKASH,PRESTIGE,PTC,QUICKHEAL,RADICO,RADIOCITY,RALLIS,RAMCOIND,RCF,RCOM,RECLTD,RICOAUTO,RNAM,RNAVAL,ROLTA,RPOWER,SADBHAV,SAIL,SALASAR,SALZERELEC,SANGHIIND,SBIN,SCI,SICAL,SINTEX,SJVN,SNOWMAN,SOUTHBANK,SPARC,SPTL,SREINFRA,SUNTECK,SUZLON,SYNDIBANK,TATACOFFEE,TATAGLOBAL,TATAMOTORS,TATAMTRDVR,TATAPOWER,TBZ,TCI,TECHM,TEJASNET,THOMASCOOK,TNPETRO,TNPL,TORNTPOWER,TRENT,TRIDENT,TV18BRDCST,TWL,UCOBANK,UJJIVAN,UNIONBANK,VAKRANGEE,VEDL,VGUARD,VIJAYABANK,WELENT,WIPRO,WONDERLA,YESBANK";
            var f = "ABAN,ABFRL,ADANIENT,ADANIPORTS,ADANIPOWER,ALBK,ALLCARGO,AMARAJABAT,AMBUJACEM,ANDHRABANK,APLLTD,APOLLOTYRE,ARVIND,ASAHIINDIA,ASHOKLEY,ATFL,AUBANK,AUROPHARMA,AXISBANK,BAJAJCORP,BAJAJELEC,BAJAJHIND,BALRAMCHIN,BANCOINDIA,BANKBARODA,BANKINDIA,BATAINDIA,BEL,BERGEPAINT,BGRENERGY,BHARATFORG,BHARTIARTL,BHEL,BIOCON,BPCL,BRNL,BSE,CADILAHC,CANBK,CANFINHOME,CAPACITE,CAPF,CASTROLIND,CDSL,CENTRALBK,CENTURYPLY,CGPOWER,CHENNPETRO,CIPLA,COALINDIA,COCHINSHIP,COFFEEDAY,COROMANDEL,COX&KINGS,CROMPTON,CUB,CUMMINSIND,DABUR,DBCORP,DBREALTY,DCBBANK,DHFL,DISHTV,DLF,EDELWEISS,EIDPARRY,EIHOTEL,ENGINERSIN,EQUITAS,ERIS,ESCORTS,EVEREADY,EXIDEIND,FEDERALBNK,FEL,FINCABLES,FORTIS,FRETAIL,GAIL,GANECOS,GATI,GDL,GEPIL,GICHSGFIN,GICRE,GLENMARK,GMRINFRA,GNA,GNFC,GODREJAGRO,GODREJCP,GODREJIND,GODREJPROP,GPPL,GRANULES,GREAVESCOT,GRUH,GSFC,GSPL,GTPL,GUJFLUORO,HAVELLS,HCC,HCLTECH,HDIL,HEIDELBERG,HEXAWARE,HIKAL,HINDALCO,HINDPETRO,HINDZINC,HOTELEELA,HSCL,HSIL,HUDCO,IBREALEST,ICICIBANK,ICICIGI,ICICIPRULI,ICIL,IDBI,IDEA,IDFC,IDFCBANK,IFCI,IGL,IGPL,IIFL,IL&FSTRANS,INDHOTEL,INDIACEM,INDIANB,INFIBEAM,INFRATEL,INOXLEISUR,INOXWIND,INTELLECT,IOB,IOC,IPCALAB,IRB,ITC,JAGRAN,JAMNAAUTO,JETAIRWAYS,JINDALPOLY,JINDALSTEL,JISLJALEQS,JKIL,JKPAPER,JKTYRE,JPASSOCIAT,JSL,JSLHISAR,JSWENERGY,JSWSTEEL,JUBILANT,JUSTDIAL,JYOTHYLAB,KAJARIACER,KALPATPOWR,KANSAINER,KARURVYSYA,KEC,KEI,KESORAMIND,KHADIM,KIRLOSENG,KOKUYOCMLN,KOTAKNIFTY,KPIT,KRBL,KSCL,KTKBANK,KWALITY,L&TFH,LALPATHLAB,LEEL,LICHSGFIN,LINDEINDIA,LOVABLE,LUPIN,M&M,M&MFIN,M100,M50,MAGMA,MAHINDCIE,MAHLOG,MAJESCO,MANALIPETC,MANAPPURAM,MANGALAM,MANINFRA,MANPASAND,MARICO,MARKSANS,MASFIN,MATRIMONY,MCLEODRUSS,MCX,MEGH,MERCATOR,MFSL,MINDTREE,MOIL,MOLDTKPAC,MOTHERSUMI,MPHASIS,MRPL,MUKANDLTD,MUTHOOTFIN,NATIONALUM,NBCC,NCC,NETWORK18,NFL,NHPC,NIACL,NIF100IWIN,NIFTYIWIN,NIITLTD,NIITTECH,NLCINDIA,NMDC,NOCIL,NRBBEARING,NTPC,OBEROIRLTY,OIL,OMAXE,ONGC,ORIENTBANK,ORIENTCEM,PARAGMILK,PCJEWELLER,PETRONET,PFC,PHOENIXLTD,PIDILITIND,PIIND,PNB,POLYPLEX,POWERGRID,PRAKASH,PRESTIGE,PTC,QUICKHEAL,RADICO,RADIOCITY,RAJESHEXPO,RALLIS,RAMCOCEM,RAMCOIND,RBLBANK,RCF,RCOM,RECLTD,RELAXO,RELCAPITAL,RELIANCE,RELINFRA,REPCOHOME,RICOAUTO,RKFORGE,RNAM,RNAVAL,ROLTA,RPOWER,SADBHAV,SAIL,SANGHIIND,SAREGAMA,SBILIFE,SBIN,SCHAND,SCI,SICAL,SINTEX,SJVN,SNOWMAN,SOBHA,SOUTHBANK,SPARC,SPTL,SREINFRA,STAR,SUNDRMFAST,SUNPHARMA,SUZLON,SYNDIBANK,SYNGENE,TATACHEM,TATACOFFEE,TATACOMM,TATAELXSI,TATAGLOBAL,TATAINVEST,TATAMOTORS,TATAMTRDVR,TATAPOWER,TATASPONGE,TATASTEEL,TBZ,TCI,TECHM,TEJASNET,THOMASCOOK,THYROCARE,TIFIN,TITAN,TNPETRO,TORNTPOWER,TRENT,TRIDENT,TV18BRDCST,TVSMOTOR,TWL,UCOBANK,UJJIVAN,UNIONBANK,UPL,VAKRANGEE,VEDL,VGUARD,VIJAYABANK,VOLTAS,WELENT,WIPRO,WOCKPHARMA,WONDERLA,YESBANK,ZEEL";
            return f.Split(',');
        }


    }
}
