# pyScript
Simple Zero Touch Node to run python scripts in a dynamo player (http://dynamobim.org).

Использование dynamo player'a как менеджера python скриптов.

Очень упрощенная вариация на тему [Revit Python Shell](https://github.com/architecture-building-systems/revitpythonshell)'a или [pyReivt](https://eirannejad.github.io/pyRevit/whatspyrevit/)'a.

## Как работает

- Нода Script.Execute находит в директории питоновский файл с таким же именем как у основного файла (.dyn) и запускает его.
- Принты и ошибки выводятся в output и доступны для просмотра из dynamo player'a.
- Текущая папка добавляется в пути для импорта
- Для создания нового скрипта не нужно открывать dynamo: достаточно просто скопировать любую пару <script name>.dyn / <script name>.py

```
folder/
├── myScript.dyn
├── myScript.py
│       from myModule import hello
│       print(hello())
└── myModule.py
        def hello():
          return 'Hello World!'
```

![Nodes](docs/images/nodes.png)
![Player](docs/images/player.png)

## Зачем

Код нужно писать с чувством собственного достоинства (IDE) а не в блокноте.

Dynamo есть везде, а возможность самостоятельно устанавливать плагины (PyRevit, RPS) есть не у всех.

Принципиально для реализации подобной схемы вообще не нужны Zero Touch ноды, но для удобства я делаю так.
