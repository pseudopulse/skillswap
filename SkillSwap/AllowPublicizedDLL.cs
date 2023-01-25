using System.Security;
using System.Security.Permissions;

//Allows you to access private methods/fields/etc from the stubbed Assembly-CSharp that is included.

[module: UnverifiableCode]
[assembly: HG.Reflection.SearchableAttribute.OptIn]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete