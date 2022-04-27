1. Параметры коммандной строки
    /i [путь к файлу] /b [размер блока]
2. Не тестировались:
- очень большие файлы. Их нет у меня.
- файловые шары и прочие сетевые дела
- внезапное исчезновение файла. но по логике - ошибка в консоль и выход...
- вообще проблема со временем с подготовкой достаточного объема тестовых сценариев налицо ) 
3. Ограничений на размер вроде бы блока нет, кроме размера long
4. Использована реализация using System.Security.Cryptography.SHA256. Она читает стрим по 4096 байт, так что, кажется, требование по памяти выполнено.
5. Постоянный порядок блоков при генерации сигнатуры соблюден. Есть юнит тест на это. Вот только гарантии, что в консоль выведется в правильном порядке нет никакой. Но и в задаче требования на порядок вывода не было. Проблема исправима, но нужно время )
6. Большие файлы в паре юнит тестов (те, которые на самом деле интеграционные тесты) - их я коммитить не стал. Уверен у вас свои есть
7. Осталась проблема с тем, что немного не так, как хотелось бы, обработается выход в случае ошибки чтения файла в процессе. Но на это уже времени нет, а требование вывести в консоль ошибку соблюдено. 
8. Остались TODO в коде, которые не имеют особого смысла справлять в текущей реализации, но в принципе было бы неплохо
