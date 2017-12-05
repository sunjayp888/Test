using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UpstoxNet;
using UpStoxWindows.Helper;

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
            upstox.Api_Key = "5ut9JIEYay4ruHyvd4PCW9QI4FzxDhMQ13Eq6ImD";
            upstox.Api_Secret = "srnlctcx79";
            upstox.Redirect_Url = "http://upstox.com";
            upstox.Stream_Mode = Mode.Full;
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
            var stockList = new List<Stock>();
            var fromDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 4, 9, 20, 0);
            var toDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 4, 9, 22, 0);
            try
            {
                var symbolsNSE = upstox.GetSymbols("NSE_EQ");
                //var batchHistory = upstox.GetHistDataBatch();
                var symbols = MISTrade().ToList();

                foreach (var symbol in symbols)
                {
                    if (symbolsNSE.Contains(symbol))
                    {
                        var data = upstox.GetHistData("NSE_EQ", symbol, "1MINUTE", fromDateTime, toDateTime, false).ToList();
                        //  var getCurrentStatus = upstox.GetISIN("NSE_EQ", symbol);

                       
                        if (data != null && data[0] != "DATETIME")
                        {
                            foreach (var symboldata in data)
                            {
                                var splitData = symboldata.Split(',');
                                if (splitData[0] != "DATETIME")
                                {
                                    stockList.Add(new Stock()
                                    {
                                        Exchange = "NSE_EQ",
                                        //Date = splitData[0],
                                        //Time = splitData[1],
                                        DateTime = DateTime.Parse(splitData[0]),
                                        Open = splitData[1],
                                        High = splitData[2],
                                        Low = splitData[3],
                                        Close = splitData[4],
                                        Volume = splitData[5],
                                        CP = splitData[6],
                                        Symbol = symbol
                                    });
                                }

                            }
                        }
                    }
                }

                var timeData = stockList.Where(d => d.DateTime >= fromDateTime && d.DateTime <= toDateTime).ToList();
                var result = new List<Stock>();
                foreach (var item in symbols)
                {
                    var symbolData = timeData.Where(d => d.Symbol.ToUpper() == item.ToUpper()).ToList();
                    if (symbolData.Any())
                    {
                        var firstItem = symbolData.FirstOrDefault();
                        var lastItem = symbolData.LastOrDefault();
                        var firstOpen = Convert.ToDouble(firstItem?.Open);
                        var lastOpen = Convert.ToDouble(lastItem?.Open);
                        if (firstOpen - lastOpen >= 1.5 ){
                            result.Add(
                                new Stock()
                                {
                                    Open = firstItem.Open,
                                    Symbol = item,
                                    DateTime = firstItem.DateTime,
                                    CP = firstItem.CP,
                                    Close = firstItem.Close,
                                    Exchange = firstItem.Exchange,
                                    High = firstItem.High,
                                    Low = firstItem.Low,
                                    Volume = firstItem.Volume,
                                    Call = "SELL"
                                });
                        }
                        if (lastOpen - firstOpen >= 1.5)
                        {
                            result.Add(
                                new Stock()
                                {
                                    Open = firstItem.Open,
                                    Symbol = item,
                                    DateTime = firstItem.DateTime,
                                    CP = firstItem.CP,
                                    Close = firstItem.Close,
                                    Exchange = firstItem.Exchange,
                                    High = firstItem.High,
                                    Low = firstItem.Low,
                                    Volume = firstItem.Volume,
                                    Call = "BUY"
                                });
                        }

                    }
                }
                //foreach (var item in timeData)
                //{
                //    var firstHalf = item.DateTime
                //}//     dataGridViewStock.AutoGenerateColumns = true;
                var list = new BindingList<Stock>(result.ToList());
                var source = new BindingSource(list, null);
                dataGridViewStock.DataSource = source;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }


        public string[] MISTrade()
        {
            var data = "3MINDIA,\r\nAARTIIND,\r\nABAN,\r\nABB,\r\nABFRL,\r\nACC,\r\nADANIENT,\r\nADANIPORTS,\r\nADANIPOWER,\r\nAJANTPHARM,\r\nAKZOINDIA,\r\nALBK,\r\nALKEM,\r\nALLCARGO,\r\nAMARAJABAT,\r\nAMBUJACEM,\r\nANDHRABANK,\r\nAPLLTD,\r\nAPOLLOHOSP,\r\nAPOLLOTYRE,\r\nARVIND,\r\nASAHIINDIA,\r\nASHOKLEY,\r\nASIANPAINT,\r\nASTRAZEN,\r\nATFL,\r\nATUL,\r\nAUBANK,\r\nAUROPHARMA,\r\nAUTOAXLES,\r\nAXISBANK,\r\nBAJAJ-AUTO,\r\nBAJAJCORP,\r\nBAJAJELEC,\r\nBAJAJFINSV,\r\nBAJAJHIND,\r\nBAJAJHLDNG,\r\nBAJFINANCE,\r\nBALKRISIND,\r\nBALRAMCHIN,\r\nBANCOINDIA,\r\nBANKBARODA,\r\nBANKBEES,\r\nBANKINDIA,\r\nBATAINDIA,\r\nBEL,\r\nBEML,\r\nBERGEPAINT,\r\nBGRENERGY,\r\nBHARATFIN,\r\nBHARATFORG,\r\nBHARTIARTL,\r\nBHEL,\r\nBIOCON,\r\nBLUEDART,\r\nBOSCHLTD,\r\nBPCL,\r\nBRITANNIA,\r\nBRNL,\r\nBSE,\r\nCADILAHC,\r\nCANBK,\r\nCANFINHOME,\r\nCAPACITE,\r\nCAPF,\r\nCASTROLIND,\r\nCDSL,\r\nCEATLTD,\r\nCENTRALBK,\r\nCENTURYPLY,\r\nCENTURYTEX,\r\nCESC,\r\nCGPOWER,\r\nCHENNPETRO,\r\nCHOLAFIN,\r\nCIPLA,\r\nCOALINDIA,\r\nCOCHINSHIP,\r\nCOFFEEDAY,\r\nCOLPAL,\r\nCONCOR,\r\nCOROMANDEL,\r\nCOX&KINGS,\r\nCRISIL,\r\nCROMPTON,\r\nCUB,\r\nCUMMINSIND,\r\nCYIENT,\r\nDABUR,\r\nDALMIABHA,\r\nDBCORP,\r\nDBREALTY,\r\nDCBBANK,\r\nDCMSHRIRAM,\r\nDHFL,\r\nDIAMONDYD,\r\nDISHTV,\r\nDIVISLAB,\r\nDIXON,\r\nDLF,\r\nDMART,\r\nDRREDDY,\r\nECLERX,\r\nEDELWEISS,\r\nEICHERMOT,\r\nEIDPARRY,\r\nEIHOTEL,\r\nEMAMILTD,\r\nENDURANCE,\r\nENGINERSIN,\r\nEQUITAS,\r\nERIS,\r\nESCORTS,\r\nEVEREADY,\r\nEXIDEIND,\r\nFEDERALBNK,\r\nFEL,\r\nFINCABLES,\r\nFORTIS,\r\nFRETAIL,\r\nGAIL,\r\nGANECOS,\r\nGATI,\r\nGDL,\r\nGEPIL,\r\nGESHIP,\r\nGICHSGFIN,\r\nGICRE,\r\nGILLETTE,\r\nGLAXO,\r\nGLENMARK,\r\nGMRINFRA,\r\nGNA,\r\nGNFC,\r\nGODREJAGRO,\r\nGODREJCP,\r\nGODREJIND,\r\nGODREJPROP,\r\nGOLDBEES,\r\nGPPL,\r\nGRANULES,\r\nGRASIM,\r\nGREAVESCOT,\r\nGRUH,\r\nGSFC,\r\nGSKCONS,\r\nGSPL,\r\nGTPL,\r\nGUJALKALI,\r\nGUJFLUORO,\r\nGUJGASLTD,\r\nHAVELLS,\r\nHCC,\r\nHCLTECH,\r\nHDFC,\r\nHDFCBANK,\r\nHDIL,\r\nHEIDELBERG,\r\nHEROMOTOCO,\r\nHEXAWARE,\r\nHGS,\r\nHIKAL,\r\nHINDALCO,\r\nHINDPETRO,\r\nHINDUNILVR,\r\nHINDZINC,\r\nHONAUT,\r\nHOTELEELA,\r\nHSCL,\r\nHSIL,\r\nHUDCO,\r\nIBREALEST,\r\nIBULHSGFIN,\r\nICICIBANK,\r\nICICIGI,\r\nICICIPRULI,\r\nICIL,\r\nIDBI,\r\nIDEA,\r\nIDFC,\r\nIDFCBANK,\r\nIEX,\r\nIFCI,\r\nIGL,\r\nIGPL,\r\nIIFL,\r\nIL&FSTRANS,\r\nINDHOTEL,\r\nINDIACEM,\r\nINDIANB,\r\nINDIGO,\r\nINDUSINDBK,\r\nINFIBEAM,\r\nINFRATEL,\r\nINFY,\r\nINOXLEISUR,\r\nINOXWIND,\r\nINTELLECT,\r\nIOB,\r\nIOC,\r\nIPCALAB,\r\nIRB,\r\nITC,\r\nJAGRAN,\r\nJAMNAAUTO,\r\nJETAIRWAYS,\r\nJINDALPOLY,\r\nJINDALSTEL,\r\nJISLJALEQS,\r\nJKCEMENT,\r\nJKIL,\r\nJKPAPER,\r\nJKTYRE,\r\nJPASSOCIAT,\r\nJSL,\r\nJSLHISAR,\r\nJSWENERGY,\r\nJSWSTEEL,\r\nJUBILANT,\r\nJUBLFOOD,\r\nJUSTDIAL,\r\nJYOTHYLAB,\r\nKAJARIACER,\r\nKALPATPOWR,\r\nKANSAINER,\r\nKARURVYSYA,\r\nKEC,\r\nKEI,\r\nKESORAMIND,\r\nKHADIM,\r\nKIRLOSENG,\r\nKOKUYOCMLN,\r\nKOTAKBANK,\r\nKOTAKNIFTY,\r\nKPIT,\r\nKRBL,\r\nKSCL,\r\nKTKBANK,\r\nKWALITY,\r\nL&TFH,\r\nLALPATHLAB,\r\nLEEL,\r\nLGBBROSLTD,\r\nLICHSGFIN,\r\nLINDEINDIA,\r\nLIQUIDBEES,\r\nLOVABLE,\r\nLT,\r\nLUPIN,\r\nM&M,\r\nM&MFIN,\r\nM100,\r\nM50,\r\nMAGMA,\r\nMAHINDCIE,\r\nMAHLOG,\r\nMAJESCO,\r\nMANALIPETC,\r\nMANAPPURAM,\r\nMANGALAM,\r\nMANINFRA,\r\nMANPASAND,\r\nMARICO,\r\nMARKSANS,\r\nMARUTI,\r\nMASFIN,\r\nMATRIMONY,\r\nMCDOWELL-N,\r\nMCLEODRUSS,\r\nMCX,\r\nMEGH,\r\nMERCATOR,\r\nMFSL,\r\nMGL,\r\nMINDAIND,\r\nMINDTREE,\r\nMOIL,\r\nMOLDTKPAC,\r\nMOTHERSUMI,\r\nMPHASIS,\r\nMRF,\r\nMRPL,\r\nMUKANDLTD,\r\nMUTHOOTFIN,\r\nNATIONALUM,\r\nNAUKRI,\r\nNBCC,\r\nNCC,\r\nNESTLEIND,\r\nNETWORK18,\r\nNFL,\r\nNH,\r\nNHPC,\r\nNIACL,\r\nNIF100IWIN,\r\nNIFTYBEES,\r\nNIFTYIWIN,\r\nNIITLTD,\r\nNIITTECH,\r\nNLCINDIA,\r\nNMDC,\r\nNOCIL,\r\nNRBBEARING,\r\nNTPC,\r\nOBEROIRLTY,\r\nOFSS,\r\nOIL,\r\nOMAXE,\r\nONGC,\r\nORIENTBANK,\r\nORIENTCEM,\r\nPAGEIND,\r\nPARAGMILK,\r\nPCJEWELLER,\r\nPEL,\r\nPERSISTENT,\r\nPETRONET,\r\nPFC,\r\nPFIZER,\r\nPGHH,\r\nPHOENIXLTD,\r\nPIDILITIND,\r\nPIIND,\r\nPNB,\r\nPNBHOUSING,\r\nPOLYPLEX,\r\nPOWERGRID,\r\nPRAKASH,\r\nPRESTIGE,\r\nPTC,\r\nPVR,\r\nQUICKHEAL,\r\nRADICO,\r\nRADIOCITY,\r\nRAJESHEXPO,\r\nRALLIS,\r\nRAMCOCEM,\r\nRAMCOIND,\r\nRAYMOND,\r\nRBLBANK,\r\nRCF,\r\nRCOM,\r\nRECLTD,\r\nRELAXO,\r\nRELCAPITAL,\r\nRELIANCE,\r\nRELINFRA,\r\nREPCOHOME,\r\nRICOAUTO,\r\nRKFORGE,\r\nRNAM,\r\nRNAVAL,\r\nROLTA,\r\nRPOWER,\r\nSADBHAV,\r\nSAIL,\r\nSALASAR,\r\nSALZERELEC,\r\nSANGHIIND,\r\nSANOFI,\r\nSAREGAMA,\r\nSBILIFE,\r\nSBIN,\r\nSCHAND,\r\nSCI,\r\nSHANKARA,\r\nSHARDAMOTR,\r\nSHRIRAMCIT,\r\nSICAL,\r\nSIEMENS,\r\nSINTEX,\r\nSIS,\r\nSJVN,\r\nSKFINDIA,\r\nSNOWMAN,\r\nSOBHA,\r\nSOLARINDS,\r\nSOUTHBANK,\r\nSPARC,\r\nSPTL,\r\nSREINFRA,\r\nSRF,\r\nSRTRANSFIN,\r\nSTAR,\r\nSUNDARMFIN,\r\nSUNDRMFAST,\r\nSUNPHARMA,\r\nSUNTECK,\r\nSUNTV,\r\nSUPREMEIND,\r\nSUZLON,\r\nSYMPHONY,\r\nSYNDIBANK,\r\nSYNGENE,\r\nTATACHEM,\r\nTATACOFFEE,\r\nTATACOMM,\r\nTATAELXSI,\r\nTATAGLOBAL,\r\nTATAINVEST,\r\nTATAMOTORS,\r\nTATAMTRDVR,\r\nTATAPOWER,\r\nTATASPONGE,\r\nTATASTEEL,\r\nTBZ,\r\nTCI,\r\nTCS,\r\nTECHM,\r\nTEJASNET,\r\nTHERMAX,\r\nTHOMASCOOK,\r\nTHYROCARE,\r\nTIFIN,\r\nTIRUMALCHM,\r\nTITAN,\r\nTNPETRO,\r\nTNPL,\r\nTORNTPHARM,\r\nTORNTPOWER,\r\nTRENT,\r\nTRIDENT,\r\nTTKPRESTIG,\r\nTV18BRDCST,\r\nTVSMOTOR,\r\nTWL,\r\nUBL,\r\nUCOBANK,\r\nUJJIVAN,\r\nULTRACEMCO,\r\nUNIONBANK,\r\nUPL,\r\nVAKRANGEE,\r\nVEDL,\r\nVENKEYS,\r\nVGUARD,\r\nVIJAYABANK,\r\nVISHNU,\r\nVOLTAS,\r\nVTL,\r\nWABCOINDIA,\r\nWELENT,\r\nWHIRLPOOL,\r\nWIPRO,\r\nWOCKPHARMA,\r\nWONDERLA,\r\nYESBANK,\r\nZEEL";
            //var data = "3MINDIA,\r\nAARTIIND";
            var trade = data.Replace("\r\n", "");
            return trade.Split(',');
        }

    }
}
