import { LinksApi } from '@/apis';

// Adjusted import path
import { LinkCards } from '@/components/LinkCards';
import { Pager } from '@/components/navigation/Pager';

export interface Props {
   searchParams?: { [key: string]: string | string[] | undefined };
}

async function getLinks(
   searchParams: { [key: string]: string | string[] | undefined } | undefined,
) {
   const page =
      searchParams && searchParams?.page
         ? parseInt(searchParams?.page as string)
         : 1;
   const tags = searchParams?.tags
      ? searchParams?.tags?.toString().split(',')
      : [];

   const linksApi = new LinksApi();
   return await linksApi.apiV1LinksGet({
      pageNo: page,
      tags: tags.join(','),
   });
}

export async function LinkResults({ searchParams }: Props) {
   const searchParameters = await searchParams;

   const links = await getLinks(searchParameters);

   const getMainContent = (): React.ReactNode => {
      return (
         <>
            <LinkCards items={links.results} />
            <Pager
               currentPage={links.pageNumber}
               totalPages={links.totalPages}
               totalResults={links.totalResults}
            />
         </>
      );
   };

   return <>{getMainContent()}</>;
}
