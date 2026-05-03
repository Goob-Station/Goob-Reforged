import subprocess
from pathlib import Path

def setup_module():
    module_name = input("Enter Module Name: ").strip()
    if not module_name:
        print("Error: ModuleName cannot be empty.")
        return

    # Assuming the script is run from a subdirectory, we go one level up for the root
    script_dir = Path(__file__).resolve().parent
    project_root = script_dir.parent

    print(f"Targeting Project Root: {project_root}")

    try:
        ## Go to /Templates/Module and install the C# template
        template_path = project_root / "Templates" / "Module"
        print(f"Installing template from: {template_path}")
        # '.' tells dotnet to install the template found in the current directory
        subprocess.run(["dotnet", "new", "install", ".", "--force"], cwd=template_path, check=True)

        # Go to /Modules and install the module instance
        modules_path = project_root / "Modules"
        modules_path.mkdir(parents=True, exist_ok=True)

        print(f"Creating new module '{module_name}' in: {modules_path}")
        subprocess.run(["dotnet", "new", "content-mod", "-n", module_name], cwd=modules_path, check=True)

        print("\nSuccessfully initialized and added module to solution.")

    except subprocess.CalledProcessError as e:
        print(f"\nAn error occurred while executing a command: {e}")
    except FileNotFoundError as e:
        print(f"\nDirectory error: {e}")

if __name__ == "__main__":
    setup_module()
