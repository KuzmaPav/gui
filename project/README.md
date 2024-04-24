# WPF Async

# Stahovač Obrázků



- **Vytváření xml elementů v C#**
	- Ve WPF lze při běhu programu upravovat xml elementy, ale i přidávat nové. *(Součástí `System.Windows.Controls` modulu)* 

- **?? operátor**
	- '??' je null-koalesční operátor, který převede hodnotu null na nějakou default hodnotu. Umožňuje rychlý převod z typu s null hodnotu na typ bez null hodnoty. `set: int? num = null` `get: num = null` -> `set: int num = null ?? 0` `get: num = 0`

- **condition ? expression1 : expression2**
	- ternární operátor. Pokud je podmínka splněná, vrátí se `expression1`, pokud ne tak `expression2`.