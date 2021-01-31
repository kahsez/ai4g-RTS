# ai4g-RTS
Unity implementation of the algorithms found in Artificial Intelligence for Games by Ian Millington and John Funge.

The demo includes a simple RTS.

Developed by Katia Galera and José Manuel Vera Menárguez

## CI/CD
I have created a workflow to [run the tests](https://github.com/kahsez/ai4g-RTS/blob/master/.github/workflows/run_tests.yml) during pull request step and other to [build](https://github.com/kahsez/ai4g-RTS/blob/master/.github/workflows/build.yml) and archive the project when merges to master. To do that I have used [Unity actions](https://github.com/game-ci/unity-actions) that uses [Unity3D docker](https://gitlab.com/gableroux/unity3d) images from [GabLeRoux](https://github.com/GabLeRoux).

## Static analyzer
I have used [Sonar cloud](https://sonarcloud.io/dashboard?id=kahsez_ai4g-RTS) as a static code analyzer to generate reports every time there is a push to the master branch. To do that I have used the [MirrorNG](https://github.com/MirrorNG/unity-runner) version of [game-ci/unity-test-runner](https://github.com/game-ci/unity-test-runner).

## Next steps

* Refactor to implement a clean architecture.
* Write tests.
* Improve Sonar reliability rating.

## Unity
Version 2020.1
