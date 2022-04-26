using System.IO;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;

Console.Write("Please enter a Username: ");
string sUser = Console.ReadLine().ToString(); // Username we are searching for

Console.Write("Please enter a Path to a Directory: ");
string directoryName = Console.ReadLine().ToString(); // Folder which we want to check

bool userFound = false; // variable to keep track if we find our user

try
{
    DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);

    DirectorySecurity dirSec = directoryInfo.GetAccessControl();
    
    foreach (FileSystemAccessRule rule in dirSec.GetAccessRules(true, true, typeof(NTAccount)))
    {
        string[] subString = rule.IdentityReference.ToString().Split('\\'); // split string DOMAIN\USERNAME
        string userName = subString[1]; // username will always be after '\'
        string groupName = userName; // Groups can also figure on the list so we set the group name to the user name 
 
        try
        {
            // Set context and find the group by the groupName
            PrincipalContext context = new PrincipalContext(ContextType.Machine);
            GroupPrincipal group = GroupPrincipal.FindByIdentity(context, groupName);

            if (group != null) // If no group is found with the name we gave the group is set to null
                               // if the group is null we skip the next step 
            {
                foreach (Principal p in group.GetMembers()) // get all group members
                {
                    UserPrincipal theUser = p as UserPrincipal;
                    // if one of the users is the user we are looking for we write it to the console
                    if (theUser.ToString() == sUser) 
                        
                    {
                        Console.WriteLine($"{sUser} has {rule.FileSystemRights} access to the folder via the group {groupName}");
                        userFound = true;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        };



        if (userName == sUser) // if we find the user , write it to the console
        {
            Console.WriteLine($"The user {userName} has following access to this folder: {rule.FileSystemRights}");
            userFound = true;
        }
    }
} catch (Exception ex)
{
    Console.Write("Exception: ");
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.StackTrace);

} finally // finally if we never find the user we also write it to the console
{
    if (!userFound)
    {
        Console.WriteLine($"{sUser} doesn't have access to {directoryName}");
    }
}


//Console.WriteLine("[{0}] - Rule {1} {2} access to {3}",
//i++,
//rule.AccessControlType == AccessControlType.Allow ? "grants" : "denies",
//rule.FileSystemRights,
//rule.IdentityReference.ToString());

