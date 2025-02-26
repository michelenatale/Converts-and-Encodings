namespace michele.natale.Converters;

public partial class Services
{
  internal class Alphabet256
  {

    //The alphabetical compilation here consists of the German, Greek, Arabic,
    //Tifinagh, Glagolitic and Armenian alphabets. It is only intended as a
    //possibility to ensure appropriate substitutions as text.
    //It does not correspond to any standardization or the like.

    public static Dictionary<byte, char> Alphabet_256 =>
      Enumerable.Range(0, 256).Select(x => ((byte)x, ToAlphabet256[x])).ToDictionary();

    public static Dictionary<char, byte> Alphabet_256R =>
      Alphabet_256.ToDictionary(x => x.Value, x => x.Key);
    public static char[] ToAlphabet256 =>
    [
      'A',  'B',  'C',  'D',  'E',  'F',  'G',  'H',  'I',  'J',  'K',  'L',  'M',  'N',  'O',  'P',  'Q',  'R',  'S',  'T',  'U',  'V',  'W',  'X',  'Y',  'Z',  'a',  'b',  'c',  'd',  'e',  'f',  'g',  'h',  'i',  'j',  'k',  'l',  'm',  'n',  'o',  'p',  'q',  'r',  's',  't',  'u',  'v',  'w',  'x',  'y',  'z',  '0',  '1',  '2',  '3',  '4',  '5',  '6',  '7',  '8',  '9',
      'Γ', 'Δ', 'Θ',  'Λ',  'Ξ',  'Π',  'Σ',  'Φ',  'Ψ',  'Ω',  'α',  'β',  'γ',  'δ',  'ε',  'ζ',  'η',  'θ',  'ι',  'κ',  'λ',  'μ',  'ν',  'ξ',  'ο',  'π',  'ρ',  'σ',  'τ',  'υ',  'φ',  'χ',  'ψ',
      'ا',  'ب',  'ت',  'ث',  'ج',  'ح',  'خ',  'د',  'ذ',  'ر',  'ز',  'س',  'ش',  'ص',  'ض',  'ط',  'ظ',  'ع',  'غ',  'ف',  'ق',  'ك',  'ل',  'م',  'ن',  'ه',  'و',  'ي',
      'ⴱ',  'ⴳ',  'ⴷ',  'ⵀ',  'ⵡ',  'ⵊ',  'ⵟ',  'ⵢ',  'ⴽ',  'ⵍ',  'ⵎ',  'ⵏ',  'ⵚ',  'ⴼ',  'ⵙ',  'ⵖ',  'ⵔ',  'ⵛ',  'ⵜ',  'ⵣ',  'ⵥ',
      'Ⰰ',  'Ⰱ',  'Ⰲ',  'Ⰳ',  'Ⰴ',  'Ⰵ',  'Ⰶ',  'Ⰷ',  'Ⰸ',  'Ⰺ',  'Ⰻ',  'Ⰼ',  'Ⰽ',  'Ⰾ',  'Ⰿ',  'Ⱀ',  'Ⱁ',  'Ⱂ',  'Ⱃ',  'Ⱄ',  'Ⱅ',  'Ⱆ',  'Ⱇ',  'Ⱈ',  'Ⱉ',  'Ⱋ',  'Ⱌ',  'Ⱍ',  'Ⱎ',  'Ⱏ',  'Ⱐ',  'Ⱑ',  'Ⱖ',  'Ⱓ',  'Ⱔ',  'Ⱗ',  'Ⱘ',  'Ⱙ',  'Ⱚ',  'Ⱛ',
      'Ա',  'Բ',  'Գ',  'Դ',  'Ե',  'Զ',  'Է',  'Ը',  'Թ',  'Ժ',  'Ի',  'Լ',  'Խ',  'Ծ',  'Կ',  'Հ',  'Ձ',  'Ղ',  'Ճ',  'Մ',  'Յ',  'Ն',  'Շ',  'Ո',  'Չ',  'Պ',  'Ջ',  'Ռ',  'Ս',  'Վ',  'Տ',  'Ր',  'Ց',  'Ւ',  'Փ',  'Ք',  'ա',  'բ',  'գ',  'դ',  'ե',  'զ',  'է',  'ը',  'թ',  'ժ',  'ի',  'լ',  'խ',  'ծ',  'կ',  'հ',  'ձ',  'ղ',  'ճ',  'մ',  'յ',  'ն',  'շ',  'ո',  'չ',  'պ',  'ջ',  'ռ',  'ս',  'վ',  'տ',  'ր',  'ց',  'ւ',  'փ',  'ք',

    ];
  }
}