# Unity Team 3 Project

Welcome to the project repository. Please follow these rules to keep our workflow clean.

## 🚀 First-Time Setup
1. **Install Git LFS:** Open your terminal and run:
   ```bash
   git lfs install
   ```
2. **Clone the Project:** Clone this repository to your local machine.
3. **Open in Unity:** Add the cloned folder to Unity Hub and open it. *(Note: Project files are currently being uploaded, please wait until the push is complete before opening! :)*

## 🛠️ Git Rules
* **No Direct Pushes:** Never push directly to `main` or `develop`.
* **Use Feature Branches:** Always create a branch for your task:
  ```bash
  git checkout develop
  git pull
  git checkout -b feature/your-task-name
  ```
* **One Person Per Scene:** Coordinate with the team before opening a scene file to prevent merge conflicts. Use **Prefabs** as much as possible.
* **Submit a PR:** Push your branch and open a Pull Request (PR) into `develop` to get your work reviewed.
