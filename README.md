# TheGrapho [RU]

Данный репозиторий содержит исходный код целевого проекта мастерской №9 "Диаграммы, или отобразить неотобразимое".

## Ученики
* Завёрткин Михаил
* Постовалов Ярослав
* Прокопенко Всеволод

## Краткое описание функционала
* Визуализация ориентированных и неориентированных графов с использованием Windows Presentation Foundation (WPF)
* Парсинг DOT файлов без потерь содержимого (full-fidelity)
* Редактирование (перемещение, переименование, добавление, удаление, выделение) вершин и рёбер
* 2 алгоритма layout'а (укладки) графов
* Сохранение графа в DOT файл
* Система инвалидации layout'а при изменении свойств графа

## Сборка
Для сборки достаточно .NET Core SDK версии 3.1 на поддерживаемой версии Windows, процесс разработки не имеет особенностей. Для справки ознакомьтесь с документацией по стандартным командам dotnet CLI.

## Сторонние лицензии
Элементы из списка ниже _явно исключаются_ из условий лицензии MPL 2.0 (см. LICENSE.txt), и на них распространяются условия исходной лицензии.
* `/TheGrapho.Parser/Utilities/StackGuard.cs` и `/TheGrapho.Parser/Utilities/TextSpan.cs` из репозитория [Roslyn](https://github.com/dotnet/roslyn), [MIT](https://github.com/dotnet/roslyn/blob/3120c07554e9df7e8db26f17b0cce544214b49d1/License.txt)
* Части `/TheGrapho/ZoomBorder.cs` из [сниппета](https://stackoverflow.com/a/6782715) с сайта Stack Overflow, [CC BY-SA 4.0](https://creativecommons.org/licenses/by-sa/4.0/) ([Wiesław Šoltés](https://stackoverflow.com/users/282801) и [Peter Boone](https://stackoverflow.com/users/10703868))
* Набор примеров графов в папке `/samples` из репозитория [graphviz](https://gitlab.com/graphviz/graphviz/-/tree/108b324b18cbadfdb3dc4041283e01c346b45ed4/graphs), [Eclipse Public License 1.0](https://gitlab.com/graphviz/graphviz/-/blob/108b324b18cbadfdb3dc4041283e01c346b45ed4/LICENSE)
* `/.gitignore` на основе [gitignore.io](https://github.com/toptal/gitignore), [MIT](https://github.com/toptal/gitignore/blob/aaefb4c9d4a2eb182e452705d43a4a93e21f4b50/LICENSE.md)

# TheGrapho [EN]

This repository contains the code and data of the final project of the 9th workshop, SSYP (LSHUP) 2020.

## Terse functionality description
* Graph and digraph visualization using Windows Presentation Foundation (WPF)
* Full-fidelity DOT file parsing
* Vertex and edge editor (move, rename, add, remove, select)
* 2 graph layout algorithms
* Saving graph to DOT file
* Graph layout invalidation on property changes

## Building
.NET Core SDK 3.1 on Windows OS is sufficient, the development process does not require any special workarounds, so use dotnet CLI Help for reference.

## Third-Party Notices
Items from the following list are _explicitly exempt_ from the terms of MPL 2.0 (see LICENSE.txt) and are covered by their original license.
* `/TheGrapho.Parser/Utilities/StackGuard.cs` and `/TheGrapho.Parser/Utilities/TextSpan.cs` from [Roslyn](https://github.com/dotnet/roslyn), [MIT](https://github.com/dotnet/roslyn/blob/3120c07554e9df7e8db26f17b0cce544214b49d1/License.txt)
* `/TheGrapho/ZoomBorder.cs` parts from Stack Overflow [code snippet](https://stackoverflow.com/a/6782715), [CC BY-SA 4.0](https://creativecommons.org/licenses/by-sa/4.0/) ([Wiesław Šoltés](https://stackoverflow.com/users/282801) and [Peter Boone](https://stackoverflow.com/users/10703868))
* A set of graph samples in `/samples` repo folder from [graphviz](https://gitlab.com/graphviz/graphviz/-/tree/108b324b18cbadfdb3dc4041283e01c346b45ed4/graphs), [Eclipse Public License 1.0](https://gitlab.com/graphviz/graphviz/-/blob/108b324b18cbadfdb3dc4041283e01c346b45ed4/LICENSE)
* `/.gitignore` based on [gitignore.io](https://github.com/toptal/gitignore), [MIT](https://github.com/toptal/gitignore/blob/aaefb4c9d4a2eb182e452705d43a4a93e21f4b50/LICENSE.md)

