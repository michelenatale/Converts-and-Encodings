
namespace michele.natale.Numerics;

partial class GF2RS
{
  /// <summary>
  /// Provides possible irreducible polynomials up to 63 degrees. 
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public static Dictionary<int, ushort[]> ToIDPs =>
    Exp_Idps.ToDictionary(x => x.Exp, x => x.Idp);

  /// <summary>
  /// Provides possible irreducible polynomials up to 14 degrees. 
  /// <para>Note: For performance reasons, the maximum exponent is 15 (32'769). The list can be extended if required.</para>
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public static (int Exp, ushort[] Idp)[] Exp_Idps =>
  [
    (1, [1]),                       //
    (2, [7]),                         //x^2+x+1
    (3, [11,13]),                     //x^3+x+1 / x^3+x^2+1
    (4, [19,25]),                     //x^4+x+1 / x^4+x^3+1
    (5, [37,41,47,55]),               //x^5+x^2+1 / x^5+x^3+1 / x^5+x^3+x^2+x+1 / x^5+x^4+x^2+x+1
    (6, [67,91,97,103]),              //x^6+x+1 / x^6+x^4+x^3+x+1 / x^6+x^5+1 / x^6+x^5+x^2+x+1
    (7, [131,137,143,145]),           //x^7+x+1 / x^7+x^3+1 / x^7+x^3+x^2+x+1 / x^7+x^4+1
    (8, [285,299,301,333]),           //x^8+x^4+x^3+x^2+1 / x^8+x^5+x^3+x+1 / x^8+x^5+x^3+x^2+1 / x^8+x^6+x^3+x^2+1
    //(9, [529,539,545,557]),           //x^9+x^4+1 / x^9+x^4+x^3+x+1 / x^9+x^5+1 / x^9+x^5+x^3+x^2+1
    //(10,[1033,1051,1063,1069]),       //x^10+x^3+1 / x^10+x^4+x^3+x+1 / x^10+x^5+x^2+x+1 / x^10+x^5+x^3+x^2+1
    //(11,[2053,2071,2091,2093]),       //x^11+x^2+1 / x^11+x^4+x^2+x+1 / x^11+x^5+x^3+x+1 / x^11+x^5+x^3+x^2+1
    //(12,[4179,4201,4219,4221]),       //x^12+x^6+x^4+x+1 / x^12+x^6+x^5+x^3+1 / x^12+x^6+x^5+x^4+x^3+x+1 / x^12+x^6+x^5+x^4+x^3+x^2+1
    //(13,[8219,8231,8245,8275]),       //x^13+x^4+x^3+x+1 / x^13+x^5+x^2+x+1 / x^13+x^5+x^4+x^2+1 / x^13+x^6+x^4+x+1
    //(14,[16427,16441,16467,16479]),   //x^14+x^5+x^3+x+1 / x^14+x^5+x^4+x^3+1 / x^14+x^6+x^4+x+1 / x^14+x^6+x^4+x^3+x^2+x+1
    //(15,[32771,32785,32791,32813]),   //x^15+x+1 / x^15+x^4+1 / x^15+x^4+x^2+x+1 / x^15+x^5+x^3+x^2+1
  ];
}
