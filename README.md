1. Параметры коммандной строки
    /i [путь к файлу] /b [размер блока]
2. Не тестировались:
- очень большие файлы. Их нет у меня.
- файловые шары и прочие сетевые дела
3. Ограничений на размер вроде бы блока нет, кроме размера long
4. Использована реализация using System.Security.Cryptography.SHA256. Она читает стрим по 4096 байт, так что, кажется, требование по памяти выполнено.
5. Остались TODO в коде, которые не имеют особого смысла справлять в текущей реализации, но в принципе было бы неплохо