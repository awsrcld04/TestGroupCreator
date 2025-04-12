using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.DirectoryServices;
using Microsoft.Win32;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Security.Principal;
using System.Reflection;

namespace TestGroupCreator
{
    class TGCMain
    {
        struct CMDArguments
        {
            public bool bParseCmdArguments;
            public string strPrincipalContextType;
            public string strTestGroupName;
            public int intNumOfTestGroups;
        }

        static void funcPrintParameterWarning()
        {
            Console.WriteLine("A parameter must be specified to run TestGroupCreator.");
            Console.WriteLine("Run TestGroupCreator -? to get the parameter syntax.");
        }

        static void funcPrintParameterSyntax()
        {
            Console.WriteLine("TestGroupCreator v1.0");
            Console.WriteLine();
            Console.WriteLine("Parameter syntax:");
            Console.WriteLine();
            Console.WriteLine("Use the following required parameters:");
            Console.WriteLine("-run                required parameter");
            Console.WriteLine("-name:              to specify the name for test groups");
            Console.WriteLine("-num:               to specify the number of test groups to create");
            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.WriteLine("TestGroupCreator -run -name:TestGroup -num:10");
        }

        static CMDArguments funcParseCmdArguments(string[] cmdargs)
        {
            CMDArguments objCMDArguments = new CMDArguments();

            objCMDArguments.strPrincipalContextType = "";
            bool bCmdArg1Complete = false;

            if (cmdargs[0] == "-run" & cmdargs.Length > 1)
            {
                if (cmdargs[1].Contains("-name:"))
                {
                    // [DebugLine] Console.WriteLine(cmdargs[1].Substring(6));
                    objCMDArguments.strTestGroupName = cmdargs[1].Substring(6);
                    bCmdArg1Complete = true;
                }
                if (bCmdArg1Complete & cmdargs[2].Contains("-num:"))
                {
                    // [DebugLine] Console.WriteLine(cmdargs[2].Substring(5));
                    objCMDArguments.intNumOfTestGroups = Int32.Parse(cmdargs[2].Substring(5));
                    objCMDArguments.bParseCmdArguments = true;
                }
            }
            else
            {
                objCMDArguments.bParseCmdArguments = false;
            }

            // remove next line if necessary; planted to remove initial error condition when creating function
            return objCMDArguments;
        }

        static void funcProgramExecution(CMDArguments objCMDArguments2)
        {
            // [DebugLine] Console.WriteLine("Entering funcProgramExecution");
            try
            {
                funcProgramRegistryTag("TestGroupCreator");

                int i = objCMDArguments2.intNumOfTestGroups;
                string newGroupName = objCMDArguments2.strTestGroupName;

                PrincipalContext ctx = funcCreatePrincipalContext(objCMDArguments2.strPrincipalContextType);

                if (ctx != null)
                {
                    for (int j = 1; j <= i; j++)
                    {
                        GroupPrincipal groupNew = new GroupPrincipal(ctx);
                        if (groupNew != null)
                        {
                            groupNew.SamAccountName = newGroupName + j.ToString();
                            groupNew.Name = newGroupName + j.ToString();
                            groupNew.DisplayName = newGroupName + j.ToString();
                            groupNew.IsSecurityGroup = true;
                            groupNew.Save();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No valid PrincipalContext.");
                }
            }
            catch (Exception ex)
            {
                MethodBase mb1 = MethodBase.GetCurrentMethod();
                funcGetFuncCatchCode(mb1.Name, ex);
            }

        }

        static void funcProgramRegistryTag(string strProgramName)
        {
            try
            {
                string strRegistryProfilesPath = "SOFTWARE";
                RegistryKey objRootKey = Microsoft.Win32.Registry.LocalMachine;
                RegistryKey objSoftwareKey = objRootKey.OpenSubKey(strRegistryProfilesPath, true);
                RegistryKey objSystemsAdminProKey = objSoftwareKey.OpenSubKey("SystemsAdminPro", true);
                if (objSystemsAdminProKey == null)
                {
                    objSystemsAdminProKey = objSoftwareKey.CreateSubKey("SystemsAdminPro");
                }
                if (objSystemsAdminProKey != null)
                {
                    if (objSystemsAdminProKey.GetValue(strProgramName) == null)
                        objSystemsAdminProKey.SetValue(strProgramName, "1", RegistryValueKind.String);
                }
            }
            catch (Exception ex)
            {
                MethodBase mb1 = MethodBase.GetCurrentMethod();
                funcGetFuncCatchCode(mb1.Name, ex);
            }
        }

        static DirectorySearcher funcCreateDSSearcher()
        {
            try
            {
                // [Comment] Get local domain context
                string rootDSE;

                System.DirectoryServices.DirectorySearcher objrootDSESearcher = new System.DirectoryServices.DirectorySearcher();
                rootDSE = objrootDSESearcher.SearchRoot.Path;
                //Console.WriteLine(rootDSE);

                // [Comment] Construct DirectorySearcher object using rootDSE string
                System.DirectoryServices.DirectoryEntry objrootDSEentry = new System.DirectoryServices.DirectoryEntry(rootDSE);
                System.DirectoryServices.DirectorySearcher objDSSearcher = new System.DirectoryServices.DirectorySearcher(objrootDSEentry);
                //Console.WriteLine(objDSSearcher.SearchRoot.Path);
                return objDSSearcher;
            }
            catch (Exception ex)
            {
                MethodBase mb1 = MethodBase.GetCurrentMethod();
                funcGetFuncCatchCode(mb1.Name, ex);
                return null;
            }
        }

        static PrincipalContext funcCreatePrincipalContext(string strContextType)
        {
            try
            {
                // [DebugLine] Console.WriteLine("Entering funcCreatePrincipalContext");
                Domain objDomain = Domain.GetComputerDomain();
                string strDomain = objDomain.Name;
                DirectorySearcher tempDS = funcCreateDSSearcher();
                string strDomainRoot = "CN=Users," + tempDS.SearchRoot.Path.Substring(7);
                // [DebugLine] Console.WriteLine(strDomainRoot);
                // [DebugLine] Console.WriteLine(strDomainRoot);

                PrincipalContext newctx = new PrincipalContext(ContextType.Domain,
                                    strDomain,
                                    strDomainRoot);

                // [DebugLine] Console.WriteLine(newctx.ConnectedServer);
                // [DebugLine] Console.WriteLine(newctx.Container);

                return newctx;

                //if (strContextType == "Domain")
                //{

                //    PrincipalContext newctx = new PrincipalContext(ContextType.Domain,
                //                                    strDomain,
                //                                    strDomainRoot);
                //    return newctx;
                //}
                //else
                //{
                //    PrincipalContext newctx = new PrincipalContext(ContextType.Machine);
                //    return newctx;
                //}
            }
            catch (Exception ex)
            {
                MethodBase mb1 = MethodBase.GetCurrentMethod();
                funcGetFuncCatchCode(mb1.Name, ex);
                return null;
            }
        }

        static void funcGetFuncCatchCode(string strFunctionName, Exception currentex)
        {
            string strCatchCode = "";

            Dictionary<string, string> dCatchTable = new Dictionary<string, string>();
            dCatchTable.Add("funcGetFuncCatchCode", "f0");
            dCatchTable.Add("funcPrintParameterWarning", "f2");
            dCatchTable.Add("funcPrintParameterSyntax", "f3");
            dCatchTable.Add("funcParseCmdArguments", "f4");
            dCatchTable.Add("funcProgramExecution", "f5");
            dCatchTable.Add("funcProgramRegistryTag", "f6");
            dCatchTable.Add("funcCreateDSSearcher", "f7");
            dCatchTable.Add("funcCreatePrincipalContext", "f8");
            dCatchTable.Add("funcCheckNameExclusion", "f9");
            dCatchTable.Add("funcMoveDisabledAccounts", "f10");
            dCatchTable.Add("funcFindAccountsToDisable", "f11");
            dCatchTable.Add("funcCheckLastLogin", "f12");
            dCatchTable.Add("funcRemoveUserFromGroup", "f13");
            dCatchTable.Add("funcToEventLog", "f14");
            dCatchTable.Add("funcCheckForFile", "f15");
            dCatchTable.Add("funcCheckForOU", "f16");
            dCatchTable.Add("funcWriteToErrorLog", "f17");
            dCatchTable.Add("funcGetUserGroups", "f18");
            dCatchTable.Add("funcOpenOutputLog", "f20");
            dCatchTable.Add("funcWriteToOutputLog", "f21");
            dCatchTable.Add("funcSearchForUser", "f22");
            dCatchTable.Add("funcSearchForGroup", "f23");
            dCatchTable.Add("funcGetGroup", "f24");
            dCatchTable.Add("funcGetUser", "f25");
            dCatchTable.Add("funcParseUserName", "f26");
            dCatchTable.Add("funcAddUserToGroup", "f27");
            dCatchTable.Add("funcGetColumnSelection", "f28");
            dCatchTable.Add("funcPrintColumnSelect", "f29");
            dCatchTable.Add("funcProcessFiles", "f30");
            dCatchTable.Add("funcCheckFileRowsForDelimiter", "f31");
            dCatchTable.Add("funcSysQueryData", "f32");
            dCatchTable.Add("funcSysQueryData2", "f33");
            dCatchTable.Add("funcGetProfileData", "f34");
            dCatchTable.Add("funcCheckOSCaptionVersion", "f35");
            dCatchTable.Add("funcRemoveProfile", "f36");
            dCatchTable.Add("funcRecurse", "f37");
            dCatchTable.Add("funcRemoveDirectory", "f38");

            if (dCatchTable.ContainsKey(strFunctionName))
            {
                strCatchCode = "err" + dCatchTable[strFunctionName] + ": ";
            }

            //[DebugLine] Console.WriteLine(strCatchCode + currentex.GetType().ToString());
            //[DebugLine] Console.WriteLine(strCatchCode + currentex.Message);

            funcWriteToErrorLog(strCatchCode + currentex.GetType().ToString());
            funcWriteToErrorLog(strCatchCode + currentex.Message);

        }

        static void funcWriteToErrorLog(string strErrorMessage)
        {
            try
            {
                FileStream newFileStream = new FileStream("Err-TestGroupCreator.log", FileMode.Append, FileAccess.Write);
                TextWriter twErrorLog = new StreamWriter(newFileStream);

                DateTime dtNow = DateTime.Now;

                string dtFormat = "MMddyyyy HH:mm:ss";

                twErrorLog.WriteLine("{0} \t {1}", dtNow.ToString(dtFormat), strErrorMessage);

                twErrorLog.Close();
            }
            catch (Exception ex)
            {
                MethodBase mb1 = MethodBase.GetCurrentMethod();
                funcGetFuncCatchCode(mb1.Name, ex);
            }

        }

        static bool funcCheckForFile(string strInputFileName)
        {
            try
            {
                if (System.IO.File.Exists(strInputFileName))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MethodBase mb1 = MethodBase.GetCurrentMethod();
                funcGetFuncCatchCode(mb1.Name, ex);
                return false;
            }
        }

        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    funcPrintParameterWarning();
                }
                else
                {
                    if (args[0] == "-?")
                    {
                        funcPrintParameterSyntax();
                    }
                    else
                    {
                        string[] arrArgs = args;
                        CMDArguments objArgumentsProcessed = funcParseCmdArguments(arrArgs);

                        if (objArgumentsProcessed.bParseCmdArguments)
                        {
                            funcProgramExecution(objArgumentsProcessed);
                        }
                        else
                        {
                            funcPrintParameterWarning();
                        } // check objArgumentsProcessed.bParseCmdArguments
                    } // check args[0] = "-?"
                } // check args.Length == 0
            }
            catch (Exception ex)
            {
                // [DebugLine] Console.WriteLine(ex.Source);
                // [DebugLine] Console.WriteLine(ex.Message);
                // [DebugLine] Console.WriteLine(ex.StackTrace);

                Console.WriteLine(ex.Message);
            }

        } // Main()
    }
}
