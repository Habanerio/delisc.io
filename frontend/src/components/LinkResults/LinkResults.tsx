import { LinksApi } from '@/apis';

// Adjusted import path
import { LinkCards } from '@/components/LinkCards';
import { Pager } from '@/components/navigation/Pager';

export interface Props {
   tagsParams?: string[];
   searchParams?: { [key: string]: string | string[] | undefined };
}

export async function LinkResults({ tagsParams, searchParams }: Props) {
   const searchParamaters = await searchParams;

   const page = searchParamaters?.page ? parseInt(searchParamaters?.page as string) : 1;
   const tags =
      tagsParams && tagsParams.length > 0
         ? tagsParams
         : searchParamaters?.tags
           ? searchParamaters?.tags?.toString().split(',')
           : [];

   const linksApi = new LinksApi(); // Create an instance
   const links = await linksApi.apiV1LinksGet({
      pageNo: page,
      tags: tags.join(','),
   });

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

   return (
      <>
         {getMainContent()}
         {/* <ContentWithRightSideBar
            main={getMainContent()}
            rightSide={<PopularRelatedTags currentTags={tags} />}
         /> */}
      </>
   );
}
