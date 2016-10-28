use gtdpad

select * from pages where deleted is null order by display_order
select * from lists where deleted is null order by page_id, display_order
select * from items where deleted is null order by list_id, display_order
