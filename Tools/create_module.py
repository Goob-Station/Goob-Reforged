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
        # Go to /Templates/Module and install the C# template
        template_path = project_root / "Templates" / "Module"
        print(f"Installing template from: {template_path}")
        # '.' tells dotnet to install the template found in the current directory
        subprocess.run(["dotnet", "new", "install", ".", "--force"], cwd=template_path, check=True)

        # Go to /Modules and install the module instance
        modules_path = project_root / "Modules"
        modules_path.mkdir(parents=True, exist_ok=True)

        print(f"Creating new module '{module_name}' in: {modules_path}")
        subprocess.run(["dotnet", "new", "content-mod", "-n", module_name, "--force"], cwd=modules_path, check=True)

        print("\nSuccessfully initialized and added module to solution.")

    except subprocess.CalledProcessError as e:
        print(f"\nAn error occurred while executing a command: {e}")
    except FileNotFoundError as e:
        print(f"\nDirectory error: {e}")

    # Add the created folder to the solution
    sln_path = project_root / "SpaceStation14.slnx"
    with open(sln_path, 'r', encoding='utf-8-sig', errors='ignore') as f:
        sln_content = f.read()

    sln_marker = "</Solution>" # Adding it to the end because whatever I don't want to mess with XML librarbies
    # The Thing
    sln_module_folder = f'''  <Folder Name="/Modules/{module_name}/">
    <Project Path="Modules/{module_name}/Content.{module_name}.Client/Content.{module_name}.Client.csproj" />
    <Project Path="Modules/{module_name}/Content.{module_name}.Common/Content.{module_name}.Common.csproj" />
    <Project Path="Modules/{module_name}/Content.{module_name}.Server/Content.{module_name}.Server.csproj" />
    <Project Path="Modules/{module_name}/Content.{module_name}.Shared/Content.{module_name}.Shared.csproj" />
    <File Path="Modules/{module_name}/module.yml" />
  </Folder>'''

    new_sln_content = sln_content.replace(sln_marker, sln_module_folder + "\n" + sln_marker)

    with open(sln_path, 'w', encoding='utf-8', newline='\n') as f:
        f.write(new_sln_content)

    # Add server and client projects to the run configuration so they build automatically
    run_path_client = project_root / ".run/Content.Client.run.xml"
    run_path_server = project_root / ".run/Content.Server.run.xml"

    with open(run_path_client, 'r', encoding='utf-8-sig', errors='ignore') as f:
        client_content = f.read()

    with open(run_path_server, 'r', encoding='utf-8-sig', errors='ignore') as f:
        server_content = f.read()

    run_marker = '      <option name="Build" />' # Adding it to the end because whatever I don't want to mess with XML librarbies

    run_config_client = f'	  <option name="Build" default="false" projectName="Content.{module_name}.Client" projectPath="$PROJECT_DIR$/Modules/{module_name}/Content.{module_name}.Client/Content.{module_name}.Client.csproj" />'

    run_config_server = f'	  <option name="Build" default="false" projectName="Content.{module_name}.Server" projectPath="$PROJECT_DIR$/Modules/{module_name}/Content.{module_name}.Server/Content.{module_name}.Server.csproj" />'

    new_run_client_content = client_content.replace(run_marker, run_config_client + "\n" + run_marker)
    new_run_server_content = server_content.replace(run_marker, run_config_server + "\n" + run_marker)

    with open(run_path_client, 'w', encoding='utf-8', newline='\n') as f:
        f.write(new_run_client_content)

    with open(run_path_server, 'w', encoding='utf-8', newline='\n') as f:
        f.write(new_run_server_content)

if __name__ == "__main__":
    setup_module()
