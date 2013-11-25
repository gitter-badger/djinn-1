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

# /example_project_dir/djinn/blueprints/nginx-box.json

    {
        "BlueprintId": "nginx-box",
        "Description": "Example blueprint for a machine running Nginx.",
        "OpenPorts":
        [
            22,
            80
        ],
        "Configure":
        [
            {
                "Name": "software",
                "Description": "Software Installation",
                "Actions":
                [
                    {
                        "__type": "Sungiant.Djinn.Command, Sungiant.Djinn",
                        "Description": "Update aptitude's package manifest",
                        "ActionContext": "Remote",
                        "IgnoreFailure": false,
                        "Value": "sudo apt-get update"
                    },
                    {
                        "__type": "Sungiant.Djinn.AptitudeInstallation, Sungiant.Djinn",
                        "Description": "Install nginx",
                        "PackageNames":
                        [
                            "nginx"
                        ]
                    },
                    {
                        "__type": "Sungiant.Djinn.Command, Sungiant.Djinn",
                        "Description": "Remove the default nginx site enabled symlink",
                        "ActionContext": "Remote",
                        "IgnoreFailure": false,
                        "Value": "sudo rm -f /etc/nginx/sites-enabled/default"
                    },
                    {
                        "__type": "Sungiant.Djinn.Command, Sungiant.Djinn",
                        "Description": "Remove the default nginx site available symlink",
                        "ActionContext": "Remote",
                        "IgnoreFailure": false,
                        "Value": "sudo rm -f /etc/nginx/sites-available/default"
                    },
                    {
                        "__type": "Sungiant.Djinn.AptitudeInstallation, Sungiant.Djinn",
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
        "Deploy":
        [
            {
                "Name": "clean",
                "Description": "clean out nginx sites",
                "Actions":
                [
                    {
                        "__type": "Sungiant.Djinn.Command, Sungiant.Djinn",
                        "Description": "Remove all nginx sites-enabled",
                        "ActionContext": "Remote",
                        "IgnoreFailure": false,
                        "Value": "sudo rm /etc/nginx/sites-enabled/*"
                    }
                ]
            },
            {
                "Name": "default-server",
                "Description": "Default Nginx Server",
                "Actions":
                [
                    {
                        "__type": "Sungiant.Djinn.NginxServerBlock, Sungiant.Djinn",
                        "Name": "default-https",
                        "Listen": "80",
                        "Locations":
                        [
                            {
                                "__type": "Sungiant.Djinn.ReturnLocationBlock, Sungiant.Djinn",
                                "Location": "/",
                                "Return": "200"
                            }
                        ]
                    }
                ]
            }
        ]
    }

# /example_project_dir/djinn/deployments/test-zone|nginx-box.json

    {
        "ZoneId": "test-zone",
        "HorizontalScale": 1,
        "BlueprintId": "nginx-box",
        "VerticalScale": 1
    }


# /example_project_dir/djinn/zones/test-zone.json

    {
        "ZoneId": "test-zone",
        "Description": "A zone used for testing."
    }

