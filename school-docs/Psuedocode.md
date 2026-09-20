# Psuedocode
>I'm not very good at writing psuedocode so I made the minimum viable amount for the project. Sorry. 

Here is some psuedocode for the Mille Project. These are not 100% runable but are provided as examples or for comperhension only. Basic stuff like config color resolving or YAML loading are not written in psuedocode.

## Program Class
This is some logic for the program class, including screen window scrolling, syntax highlighting, and bracket matching. 

### Screen Scrolling 
The following is psuedocode for the screen scrolling and to keep the curosr in line: 

```python
# Scrolling logic
if cursor_line > current_window:
	window.scroll_up(cursor_line)

elif cursor _line < bottom_edge:
	window.scroll_down(cursor_line, bottom_margin)
```

### Syntax Highlighting
The following is psuedocode for syntax highlighting:

```python
def highlight_line(text, regex_rules): 
	color_events = []
	for (rule in regex_rules):
		if (rule == regex_rule[rule]):
			start_pos = rule.find_start_point(regex_rule(rule))
			end_pos = rule.find_end_point(regex_rule(rule))
	
	color_events.sort()

	style_stack = CLEAR_COLOR
	for event in color_events:
		event.index.append(text_up)
		if (event == start_pos):
			event.color.push(style_stack)
		else:
			style_stack.pop()
		style_stack.apply(TOP_COLOR)

	return reconstructed_text
```
### Bracket Matching 
The following is psuedocode for the bracket matching system
```python
def find_matching_bracket(cursor_position):
	if cursor_position != current_bracket:
		return False
	
	direction = is_opening_bracket()
	target_bracket = get_pair(current_bracket)
	depth = 0 

	while step_cursor(direction):
		if current_char == original_bracket:
			depth = depth + 1
		elif current_char == target_bracket:
			if depth == 0:
				cursor.jump(target_bracket.position())
				return True
			depth = depth - 1

	raise Exception("Unmatched bracket error")
```