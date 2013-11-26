Installation
============

To install run:

    ./scripts/install_djinn

Then edit:

    /scripts/install_djinn_aws_credentials

so that it contains the AWS credentials you want to use.

Then run:

    ./scripts/install_djinn_aws_credentials



Example configuation:

### /example_project_dir/djinn/blueprints/nginx-box.json

    {
        "BlueprintIdentifier": "nginx-box",
        "Description": "Example blueprint for a machine running Nginx.",
        "OpenPorts":
        [
            22,
            80
        ],
        "ConfigurationActions":
        [
            {
                "Name": "software",
                "Description": "Software Installation",
                "Actions":
                [
                    {
                        "Type": "Command",
                        "Description": "Update aptitude's package manifest",
                        "IsContextRemote": true,
                        "IgnoreFailure": false,
                        "Value": "sudo apt-get update"
                    },
                    {
                        "Type": "AptitudeInstallation",
                        "Description": "Install nginx",
                        "PackageNames":
                        [
                            "nginx"
                        ]
                    },
                    {
                        "Type": "Command",
                        "Description": "Remove the default nginx site enabled symlink",
                        "IsContextRemote": true,
                        "IgnoreFailure": false,
                        "Value": "sudo rm -f /etc/nginx/sites-enabled/default"
                    },
                    {
                        "Type": "Command",
                        "Description": "Remove the default nginx site available symlink",
                        "IsContextRemote": true,
                        "IgnoreFailure": false,
                        "Value": "sudo rm -f /etc/nginx/sites-available/default"
                    },
                    {
                        "Type": "AptitudeInstallation",
                        "Description": "Install system utilities",
                        "PackageNames":
                        [
                            "nmon",
                            "htop"
                        ]
                    }
                ]
            }
        ],
        "DeploymentActions":
        [
            {
                "Name": "clean",
                "Description": "clean out nginx sites",
                "Actions":
                [
                    {
                        "Type": "Command",
                        "Description": "Remove all nginx sites-enabled",
                        "IsContextRemote": true,
                        "IgnoreFailure": false,
                        "Value": "sudo rm -f /etc/nginx/sites-enabled/*"
                    }
                ]
            },
            {
                "Name": "default-server",
                "Description": "Default Nginx Server",
                "Actions":
                [
                    {
                        "Type": "NginxServerBlock",
                        "Name": "default-http",
                        "Listen": "80",
                        "Locations":
                        [
                            {
                                "Type": "ReturnLocationBlock",
                                "Location": "/",
                                "Return": "200"
                            }
                        ]
                    }
                ]
            }
        ]
    }

### /example_project_dir/djinn/deployments/test-zone|nginx-box.json

    {
        "ZoneId": "test-zone",
        "HorizontalScale": 1,
        "BlueprintId": "nginx-box",
        "VerticalScale": 1
    }


### /example_project_dir/djinn/zones/test-zone.json

    {
        "ZoneId": "test-zone",
        "Description": "A zone used for testing."
    }

