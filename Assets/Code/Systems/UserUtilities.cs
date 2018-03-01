using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

public static class UserUtilities
{
    /// <summary> Data containing user info </summary>
    [SerializeField]
    private static UserData userData;
    /// <summary> Point Data of user </summary>
    //public static PointData UserPointData;
    /// <summary> The name of the container on the blob store </summary>
    public static string containerName = "users"; // REQUIRED-FIELD : Azure Blob Container for Users
    /// <summary> Minimum length of a password (enforced on signup) </summary>
    public static int minPassLength = 6;
    /// <summary> Minimum length of a username (enforced on signup) </summary>
    public static int minUsernameLength = 3;
    /// <summary> The exact length of a library card (no exceptions) </summary>
    public static int libraryCardLength = 14;
    /// <summary> Maximum input length of any field </summary>
    public static int MaxInputLength = 32;
    /// <summary> The currently logged in users data </summary>
    public static UserData User
    {
        get
        {
            return userData;
        }
        set
        {
            userData = value;
        }
    }
    #region BadWords
    public static List<string> badWords = new List<string>{  "a55","a55hole","aeolus","ahole","anal","analprobe","anilingus","anus"
                                            ,"areola","areole","arian","aryan","ass","assbang","assbanged","assbangs"
                                            ,"asses","assfuck","assfucker","assh0le","asshat","assho1e","ass hole,"
                                            ,"assholes","assmaster","assmunch","asswipe","asswipes","azazel","azz,"
                                            ,"b1tch","ballsack","bawdy","beaner","beardedclam","beastiality","beater,"
                                            ,"bigtits","big tits","blow job","blow","blowjob","blowjobs","bod,"
                                            ,"bodily","boink","bollock","bollocks","bollok","bone","boned","boner,"
                                            ,"boners","boobies","boobs","booby","booger","bookie,"
                                            ,"breast","breasts","bukkake","bullshit","bull shit","bullshits","bullshitted,"
                                            ,"bung","busty","butt","butt fuck","buttfuck","buttfucker","buttfucker,"
                                            ,"buttplug","cajone", "cajones","c.0.c.k","c.o.c.k.","c.u.n.t","c0ck","c-0-c-k","cahone,"
                                            ,"cameltoe","carpetmuncher","cawk","cervix","chinc","chincs","chink,"
                                            ,"chink","chode","chodes","cl1t","climax","clit","clitoris","clitorus,"
                                            ,"clits","clitty","cock","c-o-c-k","cockblock","cockholster","cockknocker,"
                                            ,"cocks","cocksmoker","cocksucker","cock sucker","commie","coon","coons,"
                                            ,"corksucker","crabs","crack","cracker","crackwhore","crap","crappy,"
                                            ,"cum","cummin","cumming","cumshot","cumshots","cumslut","cumstain,"
                                            ,"cunilingus","cunnilingus","cunny","cunt","cunt","c-u-n-t","cuntface,"
                                            ,"cunthunter","cuntlick","cuntlicker","cunts","d0ng","d0uch3","d0uche,"
                                            ,"d1ck","dago","dagos","dawgie-style","dick","dickbag","dickdipper,"
                                            ,"dickface","dickflipper","dickhead","dickheads","dickish","dick-ish,"
                                            ,"dickripper","dicksipper","dickweed","dickwhipper","dickzipper","diddle,"
                                            ,"dike","dildo","dildos","diligaf","dillweed","dimwit","dingle","dipship,"
                                            ,"doggie-style","doggy-style","dong","doofus","doosh","dopey","douch3,"
                                            ,"douche","douchebag","douchebags","douchey","dyke","dykes","ejaculate,"
                                            ,"enlargement","erect","erection","erotic","essohbee","extacy","extasy,"
                                            ,"f.u.c.k","fack","fag","fagg","fagged","faggit","faggot","fagot","fags,"
                                            ,"faig","faigt","fannybandit","felch","felcher","felching","fellate,"
                                            ,"fellatio","feltch","feltcher","fisted","fisting","fisty","floozy","foad,"
                                            ,"fondle","foobar","foreskin","freex","frigg","frigga","fubar","fuck","f-u-c-k,"
                                            ,"fuckass","fucked","fucked","fucker","fuckface","fuckin","fucking","fucknugget,"
                                            ,"fucknut","fuckoff","fucks","fucktard","fuck-tard","fuckup","fuckwad,"
                                            ,"fuckwit","fudgepacker","fuk","fvck","fxck","gae","gai","ganja","gey,"
                                            ,"gfy","ghay","ghey","gigolo","glans","goatse","godamn","godamnit","goddam,"
                                            ,"goddammit","goddamn","goldenshower","gonad","gonads","gook","gooks,"
                                            ,"gringo","gspot","g-spot","gtfo","guido","h0m0","h0mo","handjob","hard on,"
                                            ,"he11","hebe","heeb","hemp","heroin","herp","herpes","herpy,"
                                            ,"hitler","hiv","hobag","hom0","homey","homo","homoey","honky","hoor,"
                                            ,"hootch","hooter","hooters","horny","hump","humped","humping","hussy,"
                                            ,"hymen","inbred","incest","injun","j3rk0ff","jackass","jackhole","jackoff,"
                                            ,"jap","japs","jerk","jerk0ff","jerked","jerkoff","jism","jiz","jizm,"
                                            ,"jizz","jizzed","junkie","junky","kike","kikes","kinky","kkk","klan,"
                                            ,"knobend","kooch","kooches","kootch","kraut","kyke","labia","lech,"
                                            ,"leper","lesbo","lesbos","lez","lezbo","lezbos","lezzie","lezzies,"
                                            ,"lezzy","loin","loins","lube","lusty","mams","massa","masterbate,"
                                            ,"masterbating","masterbation","masturbate","masturbating","masturbation,"
                                            ,"maxi","menses","menstruate","menstruation","meth","m-fucking","mofo,"
                                            ,"molest","moolie","moron","motherfucka","motherfucker","motherfucking,"
                                            ,"mtherfucker","mthrfucker","mthrfucking","muff","muffdiver","muthafuckaz,"
                                            ,"muthafucker","mutherfucker","mutherfucking","muthrfucking","nad","nads,"
                                            ,"nappy","nazi","nazism","negro","nigga","niggah","niggas","niggaz,"
                                            ,"nigger","nigger","niggers","niggle","niglet","nimrod","ninny","nipple,"
                                            ,"nooky","nympho","opiate","opium","oral","orally","organ","orgasm,"
                                            ,"orgasmic","orgies","orgy","ovary","ovum","ovums","p.u.s.s.y.","paddy,"
                                            ,"paki","pantie","panties","panty","pastie","pasty","pcp","pecker,"
                                            ,"pedo","pedophile","pedophilia","pedophiliac","peepee","penetrate,"
                                            ,"penetration","penial","penile","penis","perversion","peyote","phalli,"
                                            ,"phallic","phuck","pillowbiter","pinko","pms","polack","pollock","poon,"
                                            ,"poontang","prick","prig","prostitute","prude","pube","pubic","pubis,"
                                            ,"punkass","punky","puss","pussies","pussy","pussypounder","puto,"
                                            ,"queaf","queef","queef","queer","queero","queers","quicky","quim,"
                                            ,"racy","rape","raped","raper","rapist","raunch","rectal","rectum,"
                                            ,"rectus","reefer","reetard","reich","retard","retarded","revue,"
                                            ,"rimjob","ritard","rtard","r-tard","rum","rump","rumprammer","ruski,"
                                            ,"s.h.i.t.","s.o.b.","s0b","sadism","sadist","scag","scantily","schizo,"
                                            ,"schlong","scrog","scrot","scrote","scrotum","scrud","scum","seaman,"
                                            ,"seamen","seduce","sh1t","s-h-1-t","shamedame","shit","s-h-i-t","shite,"
                                            ,"shiteater","shitface","shithead","shithole","shithouse","shits","shitt,"
                                            ,"shitted","shitter","shitty","shiz","sissy","skag","sleaze","sleazy,"
                                            ,"slut","slutdumper","slutkiss","sluts","smegma","smut","smutty","snatch,"
                                            ,"sniper","snuff","s-o-b","sodom","souse","soused","sperm","spic","spick,"
                                            ,"spik","spiks","spooge","spunk","stiffy","stoned","strip","stroke,"
                                            ,"stupid","suck","sucked","sucking","sumofabiatch","t1t","tard","teabagging,"
                                            ,"teat","terd","teste","testee","testes","testicle","testis","tinkle,"
                                            ,"tit","titfuck","titi","tits","tittiefucker","titties","titty","triggered"
                                            ,"tittyfuck","tittyfucker","toke","toots","tramp", "tranny","transsexual","trashy,"
                                            ,"tubgirl","turd","tush","twat","twats","unwed","urinal","urine","uterus,"
                                            ,"vag","vagina","viagra","virgin","voyeur","vulgar","vulva","wang","wank,"
                                            ,"wanker","wazoo","wedgie","weed","weenie","weewee","weiner","wetback,"
                                            ,"wh0re","wh0reface","whitey","whiz","whoralicious","whore","whorealicious,"
                                            ,"whored","whoreface","whorehopper","whorehouse","whores","whoring","wigger,"
                                            ,"woody","wop","wtf","x-rated","xxx","yeasty","yobbo","zoophile"};
    #endregion

    /// <summary> Struct for the user date (Exact: meaning this can never change after launch) </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public class UserData // Never remove anything from this structure or it will not be backwards compatible. You may add
    {
        /// <summary> Library card number associated with user </summary>
        [SerializeField]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string CardNumber;
        /// <summary> Chosen username that can that may be displayed  </summary>
        [SerializeField]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Username;
        /// <summary> Chosen password held as string only for local comparrison. This app holds no personal data </summary>
        [SerializeField]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Password;
        /// <summary> Email used only for promotional purposes </summary>
        [SerializeField]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string EMail;
        /// <summary> Current amount of points the player has </summary>
        [SerializeField]
        public int CurrentPoints;
        /// <summary> Total amount of points that the player has earned </summary>
        [SerializeField]
        public int TotalPoints;
        /// <summary> Last time this was modified </summary>
        [SerializeField]
        public long LastModified;
        /// <summary> The current status of the users account </summary>
        [SerializeField]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string Active;// Active valid inputs "Active" "Banned" "TempBan[0-9]" "Deactivated"

        public static bool operator ==(UserData ud1, UserData ud2)
        {
            return (ud1.CardNumber == ud2.CardNumber
                && ud1.Username == ud2.CardNumber
                && ud1.Password == ud2.Password
                && ud1.EMail == ud2.EMail
                && ud1.CurrentPoints == ud2.CurrentPoints
                && ud1.TotalPoints == ud2.TotalPoints
                && ud1.Active == ud2.Active);
        }

        public static bool operator !=(UserData ud1, UserData ud2)
        {
            return (ud1.CardNumber != ud2.CardNumber
                && ud1.Username != ud2.CardNumber
                && ud1.Password != ud2.Password
                && ud1.EMail != ud2.EMail
                && ud1.CurrentPoints != ud2.CurrentPoints
                && ud1.TotalPoints != ud2.TotalPoints
                && ud1.Active != ud2.Active);
        }
    }

    /// <summary> Retrieves the remote data with a specific library card number if password was correct </summary>
    public static string CheckRemoteData(string cardnum, string pass)
    {
        if (cardnum.Length > 0 && pass.Length > 0)
        {
            UserData ud;
            try
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    return "BadConnection";
                }
                ud = StringToUserData(GameManager.Azure.PullBlob(containerName, cardnum));
                if (ud.CardNumber != cardnum || !ud.Password.Equals(pass))
                {
                    return "Incorrect";
                }

                userData = ud;
                return "Correct";
            }
            catch
            {
                return "NonExistant";
            }   
        }
        return "Empty";
    }

    /// <summary> Creates the remote data for the user on the user database with given info </summary>
    public static string CreateremoteData(string userName, string libCard, string pass, string pass2, string eMail)
    {
        try
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return "BadConnection";
            }

            if (VerifyLibraryCard(libCard))
            {
                string userCheck = "";
                try
                {
                    userCheck = GameManager.Azure.PullBlob(containerName, libCard);
                }
                catch
                {
                    if (userCheck != "")
                    {
                        return "UserExists";
                    }
                    else
                    {
                        if (!(userName.Length > 0 && libCard.Length > 0 && pass.Length > 0 && pass2.Length > 0 && eMail.Length > 0))
                        {
                            return "EmptyField";
                        }
                        else if (userName.Length < minUsernameLength)
                        {
                            return "UsernameTooShort";
                        }
                        else if (!UsernameClean(userName))
                        {
                            return "UsernameIllegal";
                        }
                        else if (!PasswordMatch(pass, pass2))
                        {
                            return "MismatchedPass";
                        }
                        else if (!PasswordViability(pass))
                        {
                            return "WeakPass";
                        }
                        else if (!IsAnEMail(eMail))
                        {
                            return "InvalidEmail";
                        }
                    }

                    long now = DateTime.Now.Ticks;

                    userData = new UserData();
                    userData.CardNumber = libCard;
                    userData.Username = userName;
                    userData.Password = pass;
                    userData.EMail = eMail;
                    userData.CurrentPoints = 0;
                    userData.TotalPoints = 0;
                    userData.LastModified = now;
                    userData.Active = "New";

                    string rawData = UserDataToString(userData, true);

                    GameManager.Azure.PushBlob(containerName, libCard, rawData);

                    return "Created";
                }
            }
            else
            {
                return "InvalidCard";
            }
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.Message);
        }
        return "Error";
    }

    /// <summary> Takes in a string and returns false if it is not a viable password, true if it is </summary>
    public static bool PasswordViability(string pass)
    {
        // Less then max length
        if (pass.Length < minPassLength)
        {
            return false;
        }
        // Has a number in the string
        if (!pass.Any(c => char.IsDigit(c)))
        {
            return false;
        }
        // Has a symbol in the string
        if (!pass.Any(c => char.IsUpper(c)))
        {
            return false;
        }
        return true;
    }

    /// <summary> Serializes the data for the remember me </summary>
    public static void RememberMeSave()
    {
        try
        {
            byte[] data = UserDataToBytes(User);
            FileStream fs = new FileStream(Application.persistentDataPath + "/rem.dat", FileMode.Create);
            fs.Write(data, 0, data.Length);
            fs.Close();
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.Message);
        }
    }

    /// <summary> Deletes the remember me data if the remember me is turned off </summary>
    public static void RememberMeDelete()
    {
        try
        {
            if (RememberMeExists())
            {
                File.Delete(Application.persistentDataPath + "/rem.dat");
            }
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.Message);
        }
    }

    /// <summary> Simply returns true if there is remember me data and false if not </summary>
    public static bool RememberMeExists()
    {
        try
        {
            if (File.Exists(Application.persistentDataPath + "/rem.dat"))
            {
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.Message);
        }
        return false;
    }

    /// <summary> Retrieves and deserializes the remember me data for use if it exists </summary>
    public static void RememberMeRetrieve()
    {
        try
        {
            if (RememberMeExists())
            {
                FileStream fs = new FileStream(Application.persistentDataPath + "/rem.dat", FileMode.Open);
                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                fs.Close();
                UserData ud = BytesToUserData(data);

                userData = ud;
            }
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.Message);
        }
    }

    /// <summary> Converts a UserData to Bytes for storage </summary>
    private static byte[] UserDataToBytes(UserData ud)
    {
        try
        {
            int size = Marshal.SizeOf(ud);
            byte[] buffer = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(ud, ptr, true);
            Marshal.Copy(ptr, buffer, 0, size);
            Marshal.FreeHGlobal(ptr);

            return buffer;
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.Message);
        }
        return new byte[0];
    }

    /// <summary> Converts Bytes to UserData for use </summary>
    private static UserData BytesToUserData(byte[] bytes)
    {
        try
        { 
            UserData ud = new UserData();
            int size = Marshal.SizeOf(ud);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, ptr, size);
            ud = (UserData)Marshal.PtrToStructure(ptr, ud.GetType());
            Marshal.FreeHGlobal(ptr);
            return ud;
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e.Message);
        }
        return new UserData();
    }

    /// <summary> Saves the userdata to cloude </summary>
    public static void Save()
    {
        GameManager.Azure.PushBlob(containerName, userData.CardNumber, UserDataToString(User));
    }

    /// <summary> Removes points from user </summary>
    public static void SpendPoints(int amount)
    {
        UserData ud = User;
        ud.CurrentPoints -= amount;
        ud.LastModified = DateTime.Now.Ticks;
        User = ud;
        Save();
    }

    /// <summary> Allocates points to the userdata </summary>
    public static void AllocatePoints(int amount)
    {
        UserData ud = User;
        ud.CurrentPoints += amount;
        ud.TotalPoints += amount;
        ud.LastModified = DateTime.Now.Ticks;
        User = ud;
        Save();
    }

    /// <summary> Changes the users email and updates it in the database </summary>
    public static void ChangeEmail(string newEmail)
    {
        UserData ud = User;
        ud.EMail = newEmail;
        ud.LastModified = DateTime.Now.Ticks;
        User = ud;
        Save();
    }

    /// <summary> Changes the users username and updates it in the database </summary>
    public static void ChangeUsername(string newUsername)
    {
        UserData ud = User;
        ud.Username = newUsername;
        ud.LastModified = DateTime.Now.Ticks;
        User = ud;
        Save();
    }

    /// <summary> Changes the password email and updates it in the database </summary>
    public static void ChangePassword(string newPassword)
    {
        UserData ud = User;
        ud.Password = newPassword;
        ud.LastModified = DateTime.Now.Ticks;
        User = ud;

        if (File.Exists(Application.persistentDataPath + "/rem.dat"))
        {
            File.Delete(Application.persistentDataPath + "/rem.dat");
        }
        Save();
    }

    /// <summary> Returns true if the given strings are a match, false if not </summary>
    public static bool PasswordMatch(string pass0, string pass1)
    {
        if (!pass0.Equals(pass1))
        {
            return false;
        }
        return true;
    }

    /// <summary> Authenticates given library card with sierra server </summary>
    public static bool VerifyLibraryCard(string libCard)
    {
        RestHelper.Authenticate();
        if (libCard.Length != libraryCardLength || !RestHelper.GetUser(libCard))
        {
            return false;
        }
        return true;
    }

    /// <summary> Checks username against bad word database to ensure its not profane (if false name = profane)</summary>
    public static bool UsernameClean(string username)
    {
        bool profane = false;
        foreach (string bad in badWords)
        {
            if (username.Contains(bad))
            {
                profane = true;
            }
        }
        if (!(username.Length >= minUsernameLength) || profane)
        {
            return false;
        }
        return true;
    }

    /// <summary> Confirms that string is an email format </summary>
    public static bool IsAnEMail(string address)
    {
        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        Match match = regex.Match(address);
        if (!match.Success)
        {
            return false;
        }
        return true;
    }

    /// <summary> Trims the given sting to max card length </summary>
    public static string MaxCardLength(string libCard)
    {
        string returnString = libCard;
        if (returnString.Length > libraryCardLength)
        {
            List<char> list = returnString.ToCharArray().ToList();
            list.RemoveAt(libraryCardLength);
            returnString = new string(list.ToArray());
        }
        return returnString;
    }

    /// <summary> Trims the username to max username length </summary>
    public static string MaxUsernameLength(string username)
    {
        string returnString = username;
        if (returnString.Length > MaxInputLength)
        {
            List<char> list = returnString.ToCharArray().ToList();
            list.RemoveAt(libraryCardLength);
            returnString = new string(list.ToArray());
        }
        return returnString;
    }

    /// <summary> Trims string to max input length </summary>
    public static string MaxLength(string input)
    {
        string returnString = input;
        if (returnString.Length > MaxInputLength)
        {
            List<char> list = returnString.ToCharArray().ToList();
            list.RemoveAt(MaxInputLength);
            returnString = new string(list.ToArray());
        }
        return returnString;
    }

    // Parses the raw data and returns a UserData
    public static UserData StringToUserData(string rawData)
    {
        string rawDat = rawData;
        if (rawData[0] == '!')
        {
            UIManager.Instance.ShowEULA = true;
            rawDat = rawData.TrimStart('!');
        }

        string card = "";
        string name = "";
        string pass = "";
        string email = "";
        int current = 0;
        int total = 0;
        long lastmod = 0;
        string status = "";

        bool tagged = false;
        string stringTag = "";
        string tempString = "";
        for (int i = 0; i < rawDat.Length; i++)
        {
            if (!tagged)
            {
                stringTag += rawDat[i];
                if (rawDat[i] == '>')
                {
                    tagged = true;
                }
            }
            else
            {
                if (rawDat[i] != '<')
                {
                    tempString += rawDat[i];
                }
                if (rawDat[i] == '<' || i + 1 == rawDat.Length)
                {
                    switch (stringTag)
                    {
                        case "<CARD>":
                            card = tempString;
                            break;
                        case "<USER>":
                            name = tempString;
                            break;
                        case "<PASS>":
                            pass = tempString;
                            break;
                        case "<EMAIL>":
                            email = tempString;
                            break;
                        case "<CURRENT>":
                            current = int.Parse(tempString);
                            break;
                        case "<TOTAL>":
                            total = int.Parse(tempString);
                            break;
                        case "<LASTMOD>":
                            lastmod = long.Parse(tempString);
                            break;
                        case "<STATUS>":
                            status = tempString;
                            break;
                        default:
                            break;
                    }
                    stringTag = "<";
                    tempString = "";
                    tagged = false;
                }
            }
        }

        UserData returnData = new UserData();
        returnData.CardNumber = card;
        returnData.Username = name;
        returnData.Password = pass;
        returnData.EMail = email;
        returnData.CurrentPoints = current;
        returnData.TotalPoints = total;
        returnData.LastModified = lastmod;
        returnData.Active = status;
        return returnData;
    }

    // Encodes UserDate to string
    public static string UserDataToString(UserData user, bool newUser = false)
    {
        string rawData = (newUser)?("!"):("");
        rawData += "<CARD>" + user.CardNumber;
        rawData += "<USER>" + user.Username;
        rawData += "<PASS>" + user.Password;
        rawData += "<EMAIL>" + user.EMail;
        rawData += "<CURRENT>" + user.CurrentPoints;
        rawData += "<TOTAL>" + user.TotalPoints;
        rawData += "<LASTMOD>" + user.LastModified;
        rawData += "<STATUS>" + user.Active;
        return rawData;
    }

    // Activate user account 
    public static void ActivateAccount()
    {
        User.Active = "Active";
        Save();
    }
}