import mariadb
import csv

def dump_table_to_csv(host, user, password, database, table_name, output_file, columns):
    try:
        # Connect to MariaDB
        conn = mariadb.connect(
            host=host,
            user=user,
            password=password,
            database=database
        )
        cursor = conn.cursor()

        # Query all rows from the table
        cursor.execute(f"SELECT {columns} FROM {table_name}")
        rows = cursor.fetchall()

        # Get column names
        # column_names = [desc[0] for desc in cursor.description]
        column_names = str(columns).split(",")

        # Write to CSV
        with open(output_file, mode="w", newline="", encoding="utf-8") as f:
            writer = csv.writer(f)
            writer.writerow(column_names)  # header
            writer.writerows(rows)

        print(f"Table '{table_name}' dumped successfully to {output_file}")

    except mariadb.Error as e:
        print(f"Error: {e}")
    finally:
        if conn:
            conn.close()


# Example usage:
if __name__ == "__main__":
    dump_table_to_csv(
        host="10.0.0.52",
        user="assistant",
        password="6548",
        database="elite",
        table_name="FleetCarriers",
        output_file="carriers.csv",
        columns="system_name,carriers" #latitude,longitude,height
    )

    dump_table_to_csv(
        host="10.0.0.52",
        user="assistant",
        password="6548",
        database="elite",
        table_name="star_systems",
        output_file="systems.csv",
        columns="system_name,latitude,longitude,height"
    )
