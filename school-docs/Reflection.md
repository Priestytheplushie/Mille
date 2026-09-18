# Mille Reflection 
>By: Priestytheplushie

This is a reflection on the development process for the `Mille` CLI. Here we will cover **development experiences**, **team and technical challenges**, **language comparison** and **future language investigation**. Note: This reflection is from my prospective as the Project Manager, but the other memebers may hold differing opinions 

## Development Process
The development process for `Mille` was using a standard GitHub workflow, where you would branch off from the `main` branch, make changes, and than open a **Pull Request**. Than, another collaborator would review it and than merge it. This was intended to maintain good code quality, and it worked reasonablely well, with some caveots. 

<img width="310" height="882" alt="DevProcess" src="https://github.com/user-attachments/assets/594825ef-8836-4e76-a08e-7d508885b448" />

This development process worked rather well, because it made sure bad code didn't get merged and working on seperate branches meant everyone could work on something

### Technical Challenges
C# was a great choice for developing `Mille` because while similar to Java, it was much easier to write. There was not very many technical challenges in developing mille, it was all pretty straightforward terminal / CLI stuff 

### Team Challenges
While we didn't have very many technical challenges, we definitely had team challenges in getting everyone onto GitHub and everyone contributing to the code. Firstly, **SamLu-28** did not contribute at all to the project, instead goofing around making a weekend project that they never even showed us, which really slowed down development. He was assigned to making tests or working on multiline comment support, but neither of these features came into the final build because they have not been commited. If you check the commit history they have only made **2** commits, both to `README.md`, and both were just tests that were reverted shortly after. 

Another team challenge was getting GitHub setup, because it had some weird quirks like branches being **outdated** constantly, and loose commits like `fix typo` being commited when they probably shouldn't because they hold no real value. Going for a `Squash and Merge` workflow was something we had to learn about later on. 

## Alternative Languages
While `C#` didn't cause us many development issues, other programming languages like `Python` could have been a better fit: With C# we decided to avoid external libraries, usiong only a YAML library, but if we had used `Python` we could use their rich ecosystem, with things like `rich` or `textual`. 

An example of this is manually coding the `Display()` method and using ANSI codes to manipulate the terminal, but using Python with `textual` we could just create a `TextArea` widget which has most functionality built in. 

Besides the libraries, Python offers simpiler syntax meaning development could have been slightlly faster, although C# is more stable. Python Exceptions are also much clearer than whatever this is: 
```bash
0 Unhandled exception. System.ArgumentOutOfRangeException: Index was out of range. Must be non-negative and less than the size of the collection. (Parameter 'index')
   at System.Collections.Generic.List`1.get_Item(Int32 index)
   at Mille.Program.ProcessInput(List`1& contents, Int32& line, Int32& col, Int32& true_col, Int32& window, String expath) in C:\Users\Priesty\Downloads\Mille\Program.cs:line 164
   at Mille.Program.Main(String[] args) in C:\Users\Priesty\Downloads\Mille\Program.cs:line 96
``` 
However python is much slower than C#, so that is a consideration. So I believe that C# was the correct choice for this project and I would NOT switch languages to python if I were to do this project again. 

## Future Investigation Ideas
In the future we could investigate having Mouse Input such as selecting code with the mouse cursor and being able to press tab to move it, or able to bulk delete text. This would dramtically increase usability, but it's very complex and was out of scope for this project. 