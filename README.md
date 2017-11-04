# MD6-Curs-Project
Курсовой проект по дисциплине Защита данных.

Программная реализация алгоритма хеширования MD6.

Программа реализует алгоритм хеширования наивным образом,
именуемым разработчиками как layer-by-layer mode of operation [1, с. 47].

Программа поддерживает задание параметров алгоритма хеширования (Edit -> Hash Options).

Также программа поддерживает ввод ключа на основе парольной фразы задаваемой пользователем (По нажатию кнопки Password Key), и возможность наложить ограничение на сложность вводимого пользователем пароля (Edit -> Password Options).

Стоит отметить что после удачного ввода пароля в окне Input Password, при закрытии окна происходит немедленное хеширование пароля и в дальнейшем в программе хранится только его хеш.

Помимо этого, программа позволяет загружать ключи из файла и сохранять их в файл (кроме случая, когда ключ введен из парольной фразы).
___
[1]*Ronald Rivest, Benjamin Agre, Daniel Bailey, Christopher Crutchfield, Yevgeniy Dodis,
Kermin Elliott Fleming, Asif Khan, Jayant Krishnamurthy, Yuncheng Lin, Leo Reyzin,
Emily Shen, Jim Sukha, Drew Sutherland, Eran Tromer, and Yiqun Lisa yin. The MD6
Hash Function: a Proposal to NIST for SHA-3, 2009. (groups.csail.mit.edu/cis/md6).*
