CREATE DATABASE gtdpad
GO

USE gtdpad
GO

-- Create users

PRINT 'Creating users'

CREATE TABLE users (
    id UNIQUEIDENTIFIER NOT NULL,
    username NVARCHAR(64) NOT NULL,
    password NVARCHAR(1024) NOT NULL,
    CONSTRAINT PK_users PRIMARY KEY CLUSTERED (
        id ASC
    )
)

-- Create pages

PRINT 'Creating pages'

CREATE TABLE pages (
    id UNIQUEIDENTIFIER NOT NULL,
    user_id UNIQUEIDENTIFIER NOT NULL,
    name NVARCHAR(128) NOT NULL,
    display_order INT NOT NULL CONSTRAINT DF_pages_display_order DEFAULT 2147483647,
    created DATETIME NOT NULL CONSTRAINT DF_pages_created DEFAULT GETDATE(),
    deleted DATETIME NULL,
    CONSTRAINT PK_pages PRIMARY KEY CLUSTERED (
        id ASC
    )
)

ALTER TABLE pages WITH CHECK ADD CONSTRAINT FK_pages_users_user_id
FOREIGN KEY (user_id) REFERENCES users (id)

-- Create lists

PRINT 'Creating lists'

CREATE TABLE lists (
    id UNIQUEIDENTIFIER NOT NULL,
    page_id UNIQUEIDENTIFIER NOT NULL,
    name NVARCHAR(128) NOT NULL,
    display_order INT NOT NULL CONSTRAINT DF_lists_display_order DEFAULT 2147483647,
    created DATETIME NOT NULL CONSTRAINT DF_lists_created DEFAULT GETDATE(),
    deleted DATETIME NULL,
    CONSTRAINT PK_lists PRIMARY KEY CLUSTERED (
        id ASC
    )
)

ALTER TABLE lists WITH CHECK ADD CONSTRAINT FK_lists_pages_page_id
FOREIGN KEY (page_id) REFERENCES pages (id)

-- Create items

PRINT 'Creating items'

CREATE TABLE items (
    id UNIQUEIDENTIFIER NOT NULL,
    list_id UNIQUEIDENTIFIER NOT NULL,
    text NVARCHAR(1024) NOT NULL,
    display_order INT NOT NULL CONSTRAINT DF_items_display_order DEFAULT 2147483647,
    created DATETIME NOT NULL CONSTRAINT DF_items_created DEFAULT GETDATE(),
    deleted DATETIME NULL,
    CONSTRAINT PK_items PRIMARY KEY CLUSTERED (
        id ASC
    )
)

ALTER TABLE items WITH CHECK ADD CONSTRAINT FK_items_lists_list_id
FOREIGN KEY (list_id) REFERENCES lists (id)

GO

PRINT 'Completed Schema Generation'
SELECT 'Completed Schema Generation'

INSERT INTO users (id, username, password) VALUES ('47d2911f-c127-40c8-a39a-fb13634d2ae9', 'admin', 'admin')
INSERT INTO pages (id, user_id, name) VALUES ('55b8d142-4f1f-487c-9a29-a4f392ee3e1d', '47d2911f-c127-40c8-a39a-fb13634d2ae9', 'Default Page')

GO

