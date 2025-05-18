'use client';
import * as React from 'react';

import {
   Pagination,
   PaginationContent,
   PaginationEllipsis,
   PaginationItem,
   PaginationLink,
   PaginationNext,
   PaginationPrevious,
} from '@/components/ui/pagination';

import { usePathname } from 'next/navigation';
import { useSearchParams } from 'next/navigation';

type Props = {
   currentPage: number | undefined;
   totalPages: number | undefined;
   totalResults?: number | undefined;
};

/**
 * A simple pager component to traverse back and forth through the pages (when applicable)
 * @param props
 * @returns
 */
export function Pager(props: Props) {
   // Used to get the path of the url (excluding query strings)
   const pathName = usePathname();
   // Used to get the query strings
   const searchParams = useSearchParams();

   const tags = searchParams?.get('tags')?.replaceAll(' ', '+') ?? '';

   let baseHref = tags ? `${pathName}?tags=${tags}` : '${pathName}?';

   let query = new URLSearchParams();

   if (tags?.trim() !== '') {
      query.append('tags', tags);
   }

   if (!props.currentPage || props.currentPage < 1) {
      return null;
   }

   /**
    * Gets the Link for the first page.
    * IF, the current page is equal to 1, then the link is disabled
    */
   const getFirstPage = () => {
      const currentPage = props.currentPage ?? 1;

      const href =
         currentPage > 1
            ? query.size > 0
               ? `${pathName}?${query.toString}`
               : `${pathName}?`
            : '';

      return (
         <PaginationItem className='first'>
            <PaginationLink
               href={href}
               isActive={href !== ''}
               aria-label='Go to the first page'>
               First
            </PaginationLink>
         </PaginationItem>
      );
   };

   /**
    * Gets the Link for the previous page
    * If the current page is equal to 1, then the link is disabled
    */
   const getPreviousPage = () => {
      const currentPage = props.currentPage ?? 1;

      const href =
         currentPage > 1
            ? query.size > 0
               ? `${pathName}?page=${currentPage - 1}&${query.toString}`
               : `${pathName}?page=${currentPage - 1}`
            : '';

      return (
         <PaginationItem className='previous' style={{}}>
            <PaginationPrevious href={href} isActive={href !== ''} />
         </PaginationItem>
      );
   };

   /**
    * Gets the Link for the next page
    * If the current page is equal to the last page, then the link is disabled
    */
   const getNextPage = () => {
      const currentPage = props.currentPage ?? 1;
      const totalPages = props.totalPages ?? 1;

      const href =
         currentPage < totalPages
            ? query.size > 0
               ? `${pathName}?page=${currentPage + 1}&${query.toString}`
               : `${pathName}?page=${currentPage + 1}`
            : '';

      return (
         <PaginationItem className='next' style={{}}>
            <PaginationNext href={href} isActive={href !== ''} />
         </PaginationItem>
      );
   };

   /**
    * Gets the Link for the last page.
    * IF, the current page is equal to the last page, then the link is disabled
    */
   const lastPage = () => {
      const currentPage = props.currentPage ?? 1;
      const totalPages = props.totalPages ?? 1;

      const href =
         currentPage < totalPages
            ? query.size > 0
               ? `${pathName}?page=${totalPages}&${query.toString}`
               : `${pathName}?page=${totalPages}`
            : '';

      return (
         <PaginationItem className='last' style={{}}>
            <PaginationLink
               href={href}
               isActive={href !== ''}
               aria-label='Go to the last page'>
               Last
            </PaginationLink>
         </PaginationItem>
      );
   };

   const totalResults = props.totalResults ? (
      <li style={{ padding: '5px 10px' }}>
         Total Results: {props.totalResults}
      </li>
   ) : (
      <li style={{ padding: '5px 10px' }}></li>
   );

   return (
      <Pagination>
         <PaginationContent className='flex w-full items-center justify-between'>
            {getFirstPage()}
            {getPreviousPage()}
            <PaginationItem>
               <PaginationEllipsis />
            </PaginationItem>
            {getNextPage()}
            {lastPage()}
         </PaginationContent>
      </Pagination>
   );
}
