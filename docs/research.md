# Research

**Overview**

[Martin Odermatt](https://github.com/marodev), [Diego Marcilio](https://dvmarcilio.github.io/), and [Carlo A. Furia](https://bugcounting.net/publications.html).
Static Analysis Warnings and Automatic Fixing: A Replication for C# Projects.
In Proceedings of the 29th International Conference on Software Analysis, Evolution and Reengineering (SANER) — Reproducibility Studies and Negative Results track (RENE).


**Abstract**

Static analyzers have become increasingly popular both as developer tools and as subjects of empirical studies. Whereas static analysis tools exist for disparate programming languages, the bulk of the empirical research has focused on the popular Java programming language.
In this paper, we investigate to what extent some known results about using static analyzers for Java change when considering C# — another popular object-oriented language. To this end, we combine two replications of previous Java studies. First, we study which static analysis tools are most widely used among C# developers, and which warnings are more commonly reported by these tools on open-source C# projects. Second, we develop and empirically evaluate EagleRepair: a technique to automatically fix code in response to static analysis warnings; this is a replication of our previous work for Java.
Our replication indicates, among other things, that static code analysis is fairly popular among C# developers too; ReSharper is the most widely used static analyzer for C#; several static analysis rules are commonly violated in both Java and C# projects; automatically generating fixes to static code analysis warnings with good precision is feasible in C#.The EagleRepair tool developed for this research is available as open source.



**Cite**

    @InProceedings{OMF-SANER22, 
       author = {Martin Odermatt and Diego Marcilio and Carlo A. Furia}, 
       title = {Static Analysis Warnings and Automatic Fixing: A Replication for C\# Projects}, 
       booktitle = {Proceedings of the 29th International Conference 
       on Software Analysis, Evolution and Reengineering (SANER)}, 
       OPTpages = {XX--XX}, 
       year = {2022}, 
       month = {March}, 
       publisher = {IEEE Computer Society}, 
       note = {RENE (Reproducibility Studies and Negative Results) track}, 
       acceptancerate = {43\%} 
    }

**Download**

The paper can be downloaded [here](https://bugcounting.net/pubs/saner22-eaglerepair.pdf).