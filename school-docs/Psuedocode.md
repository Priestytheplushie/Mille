# Psuedocode
Here is some psuedocode for the Mille Project. These are not 100% runable but are provided as examples or for comperhension only.
>Not all methods in these examples exsist. 

## Colors Class 
The `Colors` class contains constants of pre-defined colors used for Synax Highlighting. It also has the `Resolve()` method which will parse the user's `HEX` code or color. 

The `Resolve(string colorInput)` method is in psuedocode below. Note that some of these methods do not exsist. 

```python 
def resolve(colorInput) 
	namedColor = colorInput.ToLowerInvariant()
	if (namedColor == "reset"):
		colorInput = Reset
	# Check against color names...

	if (namedColor !+ ""):
		return namedColor # This was a named color from the Colors class
	
	if (colorInput.stats_with("#")): # This is a HEX code
		colorInput = colorInput[1..] 
	
	# Not a real method... C# has it's own HEX parser
	if (colorInput.isHex()):
		r,g,b = colorInput.getHEX()
		return f"\x1b[38;2;{r};{g};{b}m"
	
	return "" # Fallback 
```